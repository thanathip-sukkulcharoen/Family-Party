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
    [SyncVar]
    public Vector2 Control;
    // Config 
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    //[SerializeField] Vector2 deathKick = new Vector2(25f, 25f);
    // State
    bool isAlive = true;

    // Cache component references
    PhysicsLink physicsLink;
    Animator myAnimator;
    BoxCollider2D myBodyCollider;
    CapsuleCollider2D myFeet;
    float gravityScaleAtStart;
    // Message then Method
    private void Awake()
    {
        physicsLink = GetComponent<PhysicsLink>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<BoxCollider2D>();
        myFeet = GetComponent<CapsuleCollider2D>();
        //gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Control = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        if (!isLocalPlayer) { return; }
        if (!isAlive) { return; }
        Run();
        Jump();
        FlipSprite();
        Fall();
        Crouch();
    }
    private void Run()
    {
        Vector2 playerVelocity = new Vector2(Control.x * runSpeed , physicsLink.rigidbody2D.velocity.y);
        physicsLink.Move(playerVelocity);
        bool playerHasHorizontalSpeed = Mathf.Abs(physicsLink.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
        
    }
    
    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Player"))) { return; }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0, jumpSpeed);
            physicsLink.AddVector(jumpVelocityToAdd);
            myAnimator.SetBool("Jumping", true);
        }
    }
    private void FlipSprite()
    {
        //if the player is moving horizontal
        bool playerHasHorizontalSpeed = Mathf.Abs(physicsLink.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(physicsLink.velocity.x), 1f);
        }
    }
    
    /*
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
    */
    /*
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
    */
    private void Fall()
    {
        bool isFalling = physicsLink.velocity.y < -1e-4;
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
