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
    public bool isGround, isPush;

    // Script Variables
    private PlatformerActions input;
    public GameManager gm;
    public LevelManager lm;

    // GameObject Variables
    public GameObject player;

    // RaycastHit2D Variables
    private RaycastHit2D dHit, lHit, rHit;

    // Animator Variables
    [SerializeField] private Animator playerAnimator;

    // SpriteRenderer
    [SerializeField] private SpriteRenderer spriteRenderer;

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
    }

    // Update is called once per frame
    void Update()
    {
        DrawCast();
        isPush = CheckTouching("Push Block", 1);
        isGround = CheckTouching("Ground", 1) || isPush;
        isDissolvable = CheckTouching("Dissolvable", 8);

        UpdateAboveGround();
        playerAnimator.SetFloat("VerticalForce", rb.velocity.y);
    }

    // Called once per set-time frame
    void FixedUpdate()
    {
        rb.velocity = new Vector3(horizontalMove * speed, rb.velocity.y, 0);

        // applies upward force to the object, and says its no longer jumping
        if(jump && isGround)
        {
            rb.AddForce(Vector2.up * jumpLim, ForceMode2D.Impulse);
            jump = false;
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
        if(lHit.collider != null && lHit.collider.gameObject.CompareTag(other) && pastLev)
        {
            return true;
        }

        // Determines if this object is touching a specific type of object to its right
        if(rHit.collider != null && rHit.collider.gameObject.CompareTag(other) && pastLev)
        {
            return true;
        }

        // Assumes other is not being touched
        return false;
    }

    // Updates playerAnimator to reflect whether the player is on the ground
    private void UpdateAboveGround()
    {
        // Determines if this object is touching the ground below it
        if(dHit.collider != null && dHit.collider.gameObject.CompareTag("Ground"))
        { playerAnimator.SetBool("AboveGround", false); }
        else { playerAnimator.SetBool("AboveGround", true); }
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

    // Called when the Player first touches a trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // tells the game to move to the next level
        if(other.gameObject.CompareTag("Goal")) { gm.OpenNextLevel(); }
    }

    // Dissolves bramble
    IEnumerator Dissolve(GameObject other)
    {
        Tilemap sr = other.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>();
        ParticleSystem flames = other.transform.GetChild(1).GetComponent<ParticleSystem>();
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
        flames.Stop();

        // makes sure other finished dissolving rather than player moved
        if(a <= 0) { other.SetActive(false); }
    }
}