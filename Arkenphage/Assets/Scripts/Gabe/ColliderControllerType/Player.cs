using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class Player : MonoBehaviour
{

    //Configurable Parameters
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpStrength = 5f;
    [SerializeField] float fallAnimationInitiateSpeed = 5f;


    //Player State
    bool jump = false;

    //Cached Component References
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    CapsuleCollider2D playerBodyCollider;
    CircleCollider2D playerFeetCollider;


    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<CircleCollider2D>();

    }

    void Update()
    {
        Run();
        FlipSprite();
        Jump();
        Attack();
        Falling();

    }

    private void Run()
    {
        float horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        playerRigidBody.velocity = new Vector2(horizontalInput * runSpeed, playerRigidBody.velocity.y); ;

        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon; //if player is moving
        playerAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        //Debug.Log("y velocity: " + playerRigidBody.velocity.y);


        if (!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            jump = true;
        }
        else
        {
            jump = false;
        }

        if (jump)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0, jumpStrength);
            playerRigidBody.velocity += jumpVelocityToAdd;
            playerAnimator.SetBool("Jumping", true);
        }
        else
        {
            playerAnimator.SetBool("Jumping", false);
        }
    }

    private void Falling()
    {
        bool playerIsFalling = playerRigidBody.velocity.y < -fallAnimationInitiateSpeed; //if player is moving

        if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            playerIsFalling = false;
        }

        playerAnimator.SetBool("Falling", playerIsFalling);
    }

    private void Attack()
    {
        if (!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;//can only do attack while grounded, don't keep this though? or have two separate grounded and flying attacks?
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            playerAnimator.SetTrigger("Attack");
        }
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon; //if player is moving because epsilon is the smallest float
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }
}
