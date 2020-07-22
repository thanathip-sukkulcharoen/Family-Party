using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Frog : MonoBehaviour
{
    [SerializeField] float jumpHeight = 20f;
    [SerializeField] float jumpLength = 3f;
    BoxCollider2D myBody;
    CapsuleCollider2D myFeet;
    Rigidbody2D myRigidBody;
    Animator myAnimator;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBody = GetComponent<BoxCollider2D>();
        myFeet = GetComponent<CapsuleCollider2D>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        JumpForward();
        Falling();
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

    private void JumpForward()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))){ return; }
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 jumpVelocity = new Vector2(controlThrow * jumpLength,Mathf.Abs(controlThrow) * jumpHeight);
        myRigidBody.velocity = jumpVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Jumping", playerHasHorizontalSpeed);
        
    }

    private void Falling()
    {
        bool isFalling = myRigidBody.velocity.y < -1e-6;
        myAnimator.SetBool("Falling", isFalling);
        if (isFalling)
        {
            myAnimator.SetBool("Jumping", false);
        }
    }
}
