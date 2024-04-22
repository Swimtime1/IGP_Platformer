using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    #region Variables
    
    // Float Variables
    public float speed, jumpLim, castDist;
    private float horizontalMove;

    // Rigidbody2D Variables
    public Rigidbody2D rb;

    // Boolean Variables
    private bool jump, isDissolvable, dissolving;
    public bool isGround, isPush, isWall;

    // Script Variables
    private PlatformerActions input;
    public GameManager gm;
    public LevelManager lm;

    // GameObject Variables
    public GameObject player;
    [SerializeField] private GameObject tornado;

    // RaycastHit2D Variables
    private RaycastHit2D dHit, lHit, rHit;

    // Animator Variables
    [SerializeField] private Animator playerAnimator;

    // SpriteRenderer
    [SerializeField] private SpriteRenderer spriteRenderer;

    // AudioSource Variables
    [SerializeField] private AudioSource audio;

    // AudioClip Variables
    [SerializeField] private AudioClip step, burn;

    #endregion

    // Called when the game is loaded
    private void Awake()
    {
        input = new PlatformerActions();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        jump = false;
        isGround = false;
        dissolving = false;

        tornado.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gameActive)
        {
            DrawCast();
            isPush = CheckTouching("Push Block", 1);
            isGround = CheckTouching("Ground", 0) || isPush;
            isDissolvable = CheckTouching("Dissolvable", 8);

            UpdateAboveGround();
            bool yVel = (rb.velocity.y == 0f);
            bool xVel = (rb.velocity.x == 0f);
            playerAnimator.SetBool("IsClimbing", ((yVel || isWall) && xVel));
            UpdatePushing();
        }
    }

    // Called once per set-time frame
    void FixedUpdate()
    {
        if(GameManager.gameActive)
        {
            // prevents clinging to non-climbable walls
            if(isWall && !isGround)
            {
                float yVel = (Mathf.Abs(rb.velocity.y) * -1);
                if(yVel == 0f) { yVel = -10; }
                rb.velocity = new Vector3(0, yVel, 0);
            }
            else { rb.velocity = new Vector3(horizontalMove * speed, rb.velocity.y, 0); }

            // applies upward force to the object, and says its no longer jumping
            if(jump && isGround && (lm.GetMaxLev() > 0))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                rb.AddForce(Vector2.up * jumpLim, ForceMode2D.Impulse);
                jump = false;
            }
        }
    }

    #region RaycastHit2D

    // Updates the Raycasts
    private void DrawCast()
    {
        dHit = Physics2D.Raycast(transform.position, Vector2.down, castDist);
        Debug.DrawRay(transform.position, Vector2.down * castDist, Color.red, 0f); // draws ray in scene

        lHit = Physics2D.Raycast(transform.position, Vector2.left, castDist);
        Debug.DrawRay(transform.position, Vector2.left * castDist, Color.red, 0f); // draws ray in scene

        rHit = Physics2D.Raycast(transform.position, Vector2.right, castDist);
        Debug.DrawRay(transform.position, Vector2.right * castDist, Color.red, 0f); // draws ray in scene
    }

    // Checks if the player is touching anything that affects actions
    private bool CheckTouching(string other, int lev)
    {
        bool pastLev = lm.GetMaxLev() >= lev;
        
        // Determines if this object is touching a specific type of object below it
        if(dHit.collider != null && dHit.collider.gameObject.CompareTag(other) && pastLev)
        { return true; }

        // Ensures wall jumping can occur a level after regular jumping can
        if(other == "Ground") { pastLev = lm.GetMaxLev() >= (lev + 2); }

        // Determines if this object is touching a specific type of object to its left
        if(lHit.collider != null && lHit.collider.gameObject.CompareTag(other) 
            && pastLev && spriteRenderer.flipX)
        {
            return true;
        }

        // Determines if this object is touching a specific type of object to its right
        if(rHit.collider != null && rHit.collider.gameObject.CompareTag(other) 
            && pastLev && !spriteRenderer.flipX)
        {
            return true;
        }

        // Assumes other is not being touched
        return false;
    }

    // Checks if the player is touching anything that affects actions
    private bool CheckTouchingSides(string other)
    {
        // Determines if this object is touching a specific type of object to its left
        if(lHit.collider != null && lHit.collider.gameObject.CompareTag(other) 
            && spriteRenderer.flipX)
        {
            return true;
        }

        // Determines if this object is touching a specific type of object to its right
        if(rHit.collider != null && rHit.collider.gameObject.CompareTag(other) 
            && !spriteRenderer.flipX)
        {
            return true;
        }

        // Assumes other is not being touched
        return false;
    }

    // Updates playerAnimator to reflect whether the player is on the ground
    private void UpdateAboveGround()
    {
        bool onGround = dHit.collider != null && dHit.collider.gameObject.CompareTag("Ground");
        bool onRock = dHit.collider != null && dHit.collider.gameObject.CompareTag("Push Block");
        bool onWall = dHit.collider != null && dHit.collider.gameObject.CompareTag("Wall");
        
        // Determines if this object is touching the ground below it
        if(onGround || onRock || onWall)
        { 
            playerAnimator.SetBool("AboveGround", false);
            TornadoOff(); 
        }
    }

    // Updates playerAnimator to reflect whether the player is on the ground
    private void UpdatePushing()
    {
        bool onLeft = lHit.collider != null;
        onLeft = onLeft && lHit.collider.gameObject.CompareTag("Push Block");
        onLeft = onLeft && spriteRenderer.flipX;

        bool onRight = rHit.collider != null;
        onRight = onRight && rHit.collider.gameObject.CompareTag("Push Block");
        onRight = onRight && !spriteRenderer.flipX;
        
        // Determines if this object is pushing a rock
        if((onLeft || onRight) && (rb.velocity.y == 0))
        { playerAnimator.SetBool("IsPushing", true); }
        else { playerAnimator.SetBool("IsPushing", false); }
    }

    #endregion

    // Called when the script is enabled
    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;
        input.Player.Jump.performed += OnJumpPerformed;
        input.Player.Jump.canceled += OnJumpCanceled;
        input.Player.Dissolve.performed += OnDissolvePerformed;
        input.Player.Dissolve.canceled += OnDissolveCanceled;
    }

    // Called when the script is disabled
    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.Jump.performed -= OnJumpPerformed;
        input.Player.Jump.canceled -= OnJumpCanceled;
        input.Player.Dissolve.performed -= OnDissolvePerformed;
        input.Player.Dissolve.canceled -= OnDissolveCanceled;
    }

    #region Input
    
    // Called when any of the binds associated with Move in input are used
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x;
        playerAnimator.SetFloat("HorizontalInput", horizontalMove);

        // sets direction player is facing
        if(horizontalMove > 0) { spriteRenderer.flipX = false; }
        else if(horizontalMove < 0) { spriteRenderer.flipX = true; }
    }

    // Called when any of the binds associated with Move in input stop being used
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        horizontalMove = 0f;
        playerAnimator.SetFloat("HorizontalInput", 0f);
    }

    // Called when any of the binds associated with Jump in input are used
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jump = true;
    }

    // Called when any of the binds associated with Jump in input stop being used
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        jump = false;
    }

    // Called when any of the binds associated with Dissolve in input are used
    private void OnDissolvePerformed(InputAction.CallbackContext context)
    {
        // makes sure something dissolvable is actually being touched
        if(isDissolvable && !dissolving)
        {
            dissolving = true;
            
            // Determines if this object is touching a specific type of object to its left
            if(lHit.collider != null && lHit.collider.gameObject.CompareTag("Dissolvable"))
            {
                StartCoroutine(Dissolve(lHit.collider.gameObject));
            }

            // Determines if this object is touching a specific type of object to its right
            else if(rHit.collider != null && rHit.collider.gameObject.CompareTag("Dissolvable"))
            {
                StartCoroutine(Dissolve(rHit.collider.gameObject));
            }
        }
    }

    // Called when any of the binds associated with Dissolve in input stop being used
    private void OnDissolveCanceled(InputAction.CallbackContext context)
    {
        dissolving = false;
    }

    #endregion

    // Moves the player to the start of the next level
    public void MoveLevels(Vector3 pos)
    {
        player.transform.position = pos;
    }

    #region Collisions and Triggers
    
    // Called when the Player first touches a trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // tells the game to move to the next level
        if(other.gameObject.CompareTag("Goal")) { gm.OpenNextLevel(); }
    }

    // Called when the Player enters a collision
    void OnCollisionEnter2D(Collision2D other)
    {
        isWall = other.gameObject.CompareTag("Wall");
    }

    // Called when the Player exits a collision
    void OnCollisionExit2D(Collision2D other)
    {
        bool otherIsGround = other.gameObject.CompareTag("Ground");
        bool otherIsRock = other.gameObject.CompareTag("Push Block");
        isWall = other.gameObject.CompareTag("Wall");
        
        // updates animator to JumpClimb_BlendTree
        if(otherIsGround || otherIsRock || isWall) 
        { playerAnimator.SetBool("AboveGround", true); }
    }

    #endregion

    // Dissolves bramble
    IEnumerator Dissolve(GameObject other)
    {
        Tilemap sr = other.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>();
        ParticleSystem flames = other.transform.GetChild(1).GetComponent<ParticleSystem>();
        playerAnimator.SetBool("IsBurning", true);
        audio.Play();
        flames.Play();
        
        float r = sr.color.r;
        float g = sr.color.g;
        float b = sr.color.b;
        float a = sr.color.a * 255;

        // fades the bramble
        while((a > 0) && isDissolvable && dissolving)
        {
            a -= 1f;
            sr.color = new Color(r, g, b, (a / 255f));
            yield return new WaitForSeconds(0.01f);
        }

        dissolving = false;
        playerAnimator.SetBool("IsBurning", false);
        flames.Stop();
        audio.Stop();

        // makes sure other finished dissolving rather than player moved
        if(a <= 0) { other.SetActive(false); }
    }

    #region Audio
    
    // Simply plays a walking sound
    public void PlayWalk()
    {
        audio.PlayOneShot(step);
    }

    #endregion

    #region Tornado

    // Turns on the tornado
    public void TornadoOn()
    {
        /* StartCoroutine(TornadoSpin()); */
        tornado.SetActive(true);
    }

    // Turns off the tornado
    public void TornadoOff()
    {
        tornado.SetActive(false);
    }

    // Rapidly spins the Tornado
    private IEnumerator TornadoSpin()
    {
        SpriteRenderer ts = tornado.GetComponent<SpriteRenderer>();
        
        while(tornado.activeInHierarchy)
        {
            ts.flipX = !ts.flipX;
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("End TornadoSpin()");
    }

    #endregion
}