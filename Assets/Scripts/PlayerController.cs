using Cinemachine;
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
    [SyncVar] Vector2 CrossControl;
    // Set Constants because somehow when set float = 0, still greater than Mathf.Epsilon
    private const float Epsilon = 1e-3f;
    // Config 
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    //[SerializeField] float climbSpeed = 5f;
    //[SerializeField] Vector2 deathKick = new Vector2(25f, 25f);
    // State
    bool isAlive = true;

    // Cache component references
    Rigidbody2D rigidbody2D;
    BoxCollider2D bodyCollider;
    CapsuleCollider2D feet;
    Animator animator;
    public RectTransform Display;
    public RectTransform playerName;
    [SerializeField] private AudioClip jumpSFX;
    //float gravityScaleAtStart;
    // Message then Method
    public override void OnStartAuthority()
    {
        SetupCinemachineCamera();
    }

    private void SetupCinemachineCamera()
    {
        if (!hasAuthority) { return; }
        FindObjectOfType<CinemachineVirtualCamera>().Follow = this.gameObject.transform;
    }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        feet = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        //gravityScaleAtStart = myRigidBody.gravityScale;
    }

    void Update()
    {
        FlipChatText();
        FlipPlayerName();
        if (!hasAuthority) { return; }
        if (!isAlive) { return; }
        CrossControl = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        Run();
        Jump();
        FlipSprite();
        Fall();
        Crouch();
    }
    private void Run()
    {
        Vector2 runVelocity = new Vector2(CrossControl.x * runSpeed, rigidbody2D.velocity.y);
        rigidbody2D.velocity = runVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x) > Epsilon;
        animator.SetBool("Running", playerHasHorizontalSpeed);
    }
    public void Jump()
    {
        if (!feet.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Player"))) { return; }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocity = new Vector2(0, jumpSpeed);
            rigidbody2D.velocity += jumpVelocity;
            animator.SetBool("Jumping", true);
            TriggerJumpSFX();
        }
    }
    
    private void FlipSprite()
    {
        //if the player is moving horizontal
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x) > Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidbody2D.velocity.x), 1f);
        }
    }
    private void FlipChatText()
    {
        Display.localScale = new Vector2(Mathf.Sign(transform.localScale.x), 1f);
    }

    private void FlipPlayerName()
    {
        playerName.localScale = new Vector2(Mathf.Sign(transform.localScale.x), 1f);
    }

    private void Fall()
    {
        bool isFalling = rigidbody2D.velocity.y < -1 * Epsilon;
        animator.SetBool("Falling", isFalling);
        if (isFalling)
        {
            animator.SetBool("Jumping", false);
        }
    }
    private void Crouch()
    {
        bool isPressDown = CrossControl.y < -1 * Epsilon;
        animator.SetBool("Crouching", isPressDown);
    }
    
    private void TriggerJumpSFX()
    {
        AudioSource.PlayClipAtPoint(jumpSFX, this.transform.position);
        
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
}
