using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    
    // Float Variables
    public float speed, jumpLim, castDist;
    private float horizontalMove;

    // Rigidbody2D Variables
    public Rigidbody2D rb;

    // Boolean Variables
    private bool jump, isDissolvable;
    public bool isGround;

    // Script Variables
    private PlatformerActions input;
    public GameManager gm;
    public LevelManager lm;

    // GameObject Variables
    public GameObject player;

    // RaycastHit2D Variables
    private RaycastHit2D dHit, lHit, rHit;

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
    }

    // Update is called once per frame
    void Update()
    {
        DrawCast();
        isGround = CheckTouching("Ground", 1);
        isDissolvable = CheckTouching("Dissolvable", 10);
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

    // Checks if the player can jump
    private bool CheckTouching(string other, int lev)
    {
        bool pastLev = lm.currLev >= lev;
        
        // Determines if this object is touching a specific type of object below it
        if(dHit.collider != null && dHit.collider.gameObject.CompareTag(other) && pastLev)
        { return true; }

        // Ensures wall jumping can occur a level after regular jumping can
        if(other == "Ground") { pastLev = lm.currLev >= (lev + 2); }

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
    }

    // Called when any of the binds associated with Move in input stop being used
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        horizontalMove = 0f;
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
        Debug.Log("Dissolving");
    }

    // Called when any of the binds associated with Dissolve in input stop being used
    private void OnDissolveCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Stopped Dissolving");
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
}