using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    //Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    //State
    bool isAlive = true;

    // Cached component references.
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeet;
    float gravityScaleAtStart;


    //Message then methods
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        myFeet = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Run();
        FlipSprite();
        Jump();
        ClimbLadder();
        Die();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // accessing the CPIM "Horizontal" axis input and storing it. It's value is between -1 to +1.

        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y); // set up new Vector 2 with the x being the above as we will use the input manager to control the x.
                                                                                    // and we set the y to "myRigidBody" which has been set in the start method to whatever it's own y is.

        myRigidBody.velocity = playerVelocity; // set the overall velocity to the Vector 2 set up above.

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; //use this to give "playerHasHorizontalSpeed" a true or false value - USED BELOW, SEE EXPLANATION.
        myAnimator.SetBool("Running", playerHasHorizontalSpeed); // access the animator and set the bool param to either true or false based on the above.
    }

    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; } // this stops our player from jumping continuously by checking if it is touching our "Ground" layer.

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);

    }

    private void FlipSprite() // our character faces right when the x-scale is set to one and it will face left if set to -1.
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        // "Matchf.Abs" returns the value as an "absolute" which means that no matter what the figure is, it returns the positive value of that figure. So whether the figure is 10 or -10, it
        // will return +10. We use this here because we want to know if our player is moving at all on the x axis by asking if our rigid body's velocity is higher than zero. 
        // "myRigidBody.velocity.x" will return a positive figure if we're moving in right on the x axis but a negative figure if moving left. Mathf.Abs will ensure our
        // value is a positive figure regardless of which direction we are travelling. 
        // "Mathf.Epsilon" means "The smallest value that a float can have different from zero." We use this so any amount more than zero will be counted no matter how small it is
        // This is important because it might normally only count 1 as being higher than 0. So if our value was 0.05, if wouldnt count. "Mathf.Epsilon" removes that issue.

        if (playerHasHorizontalSpeed) // so if "myRigisBody.velocity.x" is higher than zero.
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
            // "transform.localScale" is the "Scale" part of our transform which dictates the size of our object. We want this because we want to set the scale to either 1 or -1.
            // "Mathf.Sign" will return 1 if the value is a positive figure and -1 if it's a negative figure e.g. "45" would return a 1 and "-172" would return a -1.
            // So we set the x value of our "localScale" to 1 if our x's velocity is a positive amount (moving right) and -1 if it's a negative amount (moving left). Y stays at 1.
        }
    }

    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Death");
            GetComponent<Rigidbody2D>().velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }


}
