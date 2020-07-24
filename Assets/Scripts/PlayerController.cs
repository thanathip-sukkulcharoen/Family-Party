using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : NetworkBehaviour
{
    /*
    Rigidbody2D rigidbody2D;
    public float speed;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        // only simulate body on server
        rigidbody2D.simulated = true;
    }
    
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            rigidbody2D.velocity += new Vector2(0, CrossPlatformInputManager.GetAxis("Vertical") * speed * Time.fixedDeltaTime);
        }
    }
    */

    
    
    // Config 
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);
    // State
    bool isAlive = true;

    // Cache component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    BoxCollider2D myBodyCollider;
    CapsuleCollider2D myFeet;
    float gravityScaleAtStart;
    // Message then Method
    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<BoxCollider2D>();
        myFeet = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }
        if (!isAlive) { return; }
        Run();
        //RunUp();
        
        Jump();
        FlipSprite();
        ClimbLadder();
        Fall();
        Crouch();
        Die();
        
    }
    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical"); // -1 to +1
        Vector2 playerVelocity = new Vector2(0f, controlThrow * runSpeed * Time.deltaTime);
        GetComponent<PhysicsLink>().ApplyForce(playerVelocity, ForceMode2D.Impulse);
       // bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
       // myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }
    private void RunUp()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical"); // -1 to +1
        Vector2 playerVelocity = new Vector2(0f, controlThrow * runSpeed);
        GetComponent<PhysicsLink>().ApplyForce(playerVelocity, ForceMode2D.Force);
        // bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        // myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }
    
    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            myRigidBody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("Climbing", false);
            return; 
        }
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }
    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground","Platform"))) { return; }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity = jumpVelocityToAdd;
            myAnimator.SetBool("Jumping", true);
        }
    }
    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy","Hazards")))
        {
            myAnimator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
            isAlive = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
    private void FlipSprite()
    {
        //if the player is moving vertically
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }
    private void Fall()
    {
        bool isFalling = myRigidBody.velocity.y < -1e-4;
        myAnimator.SetBool("Falling", isFalling);
        if (isFalling)
        {
            myAnimator.SetBool("Jumping", false);
        }
    }
    private void Crouch()
    {
        bool isPressDown = CrossPlatformInputManager.GetAxis("Vertical") < 0;
        myAnimator.SetBool("Crouching", isPressDown);
    }
   
    
}
