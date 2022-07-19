using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    Rigidbody2D myRigidBody;
    [SerializeField] float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFacingRight())
        {
            myRigidBody.velocity = new Vector2(moveSpeed, 0); // if returns true, this will move the enemy to the right.
        }
        else
        {
            myRigidBody.velocity = new Vector2(-moveSpeed, 0); // if returns false, this will move the enemy to the left.
        }
 
    }

    bool isFacingRight() // if our enemy is moving right it's x will be more than 0 so this will return true. if moving left it will be less than zero so will return false 
    {
        return transform.localScale.x > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f); // Same as our player script this will change the enemy's scale to either 1 or -1 (facing right or left)
        // We put a minus before the "(Mathf..." however because then what ever it currently is when it collides, this will make it the opposite. So if the enemy is currently moving right it's 
        // velocity.x will be 1 so the above will change it to -1 and if it's moving left it's velocity.x will be -1 so this will change it to 1
    }

}
