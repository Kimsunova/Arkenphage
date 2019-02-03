using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

//public enum PlayerState
//{
//    walk,
//    attack,
//    interact,
//    stagger,
//    idle,
//    dead,
//    falling,
//    grappling
//}

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class Player : MonoBehaviour
{

    //Configurable Parameters
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpStrength = 5f;
    [SerializeField] float fallAnimationInitiateSpeed = 5f;
    [SerializeField] float fallMoveSpeed = 5f;



    //Player State
    //public PlayerState currentState;
    public FloatValue currentHealth;
    public Signal playerHealthSignal;
    public Signal playerHit;



    public bool jump = false;
    public bool IsDead = false;
    public bool IsGrappling = false;
    public bool IsMoving = false;
    public bool IsFalling = false;
    //bool IsIdle = false;
    public bool IsAttacking = false;
    public bool IsStaggered = false;
    public bool IsInteracting = false;

    //public bool IsGrappling { get; set; }//should i use these properties like this?


    //Cached Component References
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    CapsuleCollider2D playerBodyCollider;
    CircleCollider2D playerFeetCollider;
    PolygonCollider2D attackHitBox;

    public Interactable interactFocus;



    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<CircleCollider2D>();
        //currentState = PlayerState.walk;
        //IsIdle = true;
        attackHitBox = GetComponentInChildren<PolygonCollider2D>();

    }

    private void FixedUpdate()
    {
        

        //new
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && !IsAttacking && !IsStaggered)
        {
            StartCoroutine(AttackCo());
        }
        else if (!IsDead && !IsGrappling && !IsStaggered)//need falling? should be able to move while falling?
        {
            //UpdateAnimationAndMove();
            Run();
        }
    }

    void Update()
    {

        if (IsInteracting || IsDead || IsStaggered)
        {
            return;//this isn't working and player can still move around when state is dead
        }

        if (Input.GetKeyDown(KeyCode.P) && interactFocus != null)
        {
            interactFocus.Interact();
        }


        DropDown();


        FlipSprite();//flipsprite is interfereing with the new rope system script test so just commenting out temporarily


        Jump();
        //Attack();
        Falling();//this still triggers when dead?, like if you die when in the falling animation (see falling into spike pit)
        HazardDeath();

    }

    private void Run()
    {
        float horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        playerRigidBody.velocity = new Vector2(horizontalInput * runSpeed, playerRigidBody.velocity.y);

        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon; //if player is moving

        if (playerHasHorizontalSpeed)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }

        playerAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        //Debug.Log("y velocity: " + playerRigidBody.velocity.y);


        if (!(playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround"))))
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

        if (IsDead)
        {
            playerAnimator.SetBool("Falling", false);
            return;
        }

        if (playerRigidBody.velocity.y < -fallAnimationInitiateSpeed)
        {
            
                IsFalling = true;
            
        }

        if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")))
        {
            IsFalling = false;
        }


        //if(currentState == PlayerState.falling)
        //{
        //    float horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        //    playerRigidBody.velocity += new Vector2(horizontalInput * fallMoveSpeed, 0f);
        //}

        playerAnimator.SetBool("Falling", IsFalling || IsGrappling);
    }

    private void DropDown()
    {
        if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")))
        {

            float verticalInput = CrossPlatformInputManager.GetAxisRaw("Vertical");

            if (verticalInput < 0 && CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                StartCoroutine(DropCo());
            }
        }

    }

    private IEnumerator DropCo()
    {
        playerFeetCollider.enabled = false;
        playerBodyCollider.enabled = false;
        yield return new WaitForSeconds(.3f);
        playerFeetCollider.enabled = true;
        playerBodyCollider.enabled = true;
    }

    //private void Attack()
    //{

    //    //old
    //    if (!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
    //    {
    //        return;//can only do attack while grounded, don't keep this though? or have two separate grounded and flying attacks?
    //    }

    //    if (CrossPlatformInputManager.GetButtonDown("Fire1"))
    //    {
    //        playerAnimator.SetTrigger("Attack");
    //    }
    //    //end old


    //}

    private void FlipSprite()
    {
        if (IsStaggered || IsDead || IsAttacking)//****add grappling for compatibility with new ope system
            return;//so player doesn't turn around when staggered or killed

        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon; //if player is moving because epsilon is the smallest float
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }


    private IEnumerator AttackCo()
    {
        //new
        playerAnimator.SetTrigger("Attack");
        IsAttacking = true;


        //playerAnimator.SetBool("attacking", true);//should be trigger and no need for coroutine?
        //currentState = PlayerState.attack;
        //yield return null;//waits one frame
        //playerAnimator.SetBool("attacking", false);//so it won't go back in here (why not just use a trigger type?)
        yield return new WaitForSeconds(.3f);//this still needed?

        IsAttacking = false;
    }

    //public void RaiseItem()
    //{
    //    if (playerInventory.currentItem != null)
    //    {
    //        if (currentState != PlayerState.interact)
    //        {
    //            playerAnimator.SetBool("receive item", true);
    //            currentState = PlayerState.interact;
    //            receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
    //        }
    //        else
    //        {
    //            playerAnimator.SetBool("receive item", false);
    //            currentState = PlayerState.idle;
    //            receivedItemSprite.sprite = null;
    //            playerInventory.currentItem = null;
    //        }
    //    }
    //}

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            playerHit.Raise();
            playerAnimator.SetBool("Stagger", true);
            StartCoroutine(KnockCo(knockTime));
        }
        else
        {
            StartCoroutine(DeathEffect());
        }
    }

    private void HazardDeath()
    {

        if (playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))//its not detectingthe layer mask
        {
            StartCoroutine(DeathEffect());
        }
    }

    private IEnumerator DeathEffect()
    {
        playerAnimator.SetBool("Falling", false);
        playerAnimator.SetBool("IsDead", true);//why will this not play from falling?
        IsDead = true;//i want to be able to set player state to dead here but it will prevent the death animation from happening sometimes, like maybe when falling?
        IsDead = true;
        //Destroy(this.gameObject, 3f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//should create separate methods or even classes for scenemanagement later once that is worked out, right now just reloads on player death for debugging
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (playerRigidBody != null)
        {
            yield return new WaitForSeconds(knockTime);
            playerRigidBody.velocity = Vector2.zero;
            playerAnimator.SetBool("Stagger", false);
            IsStaggered = false;
            IsMoving = false;//set back to idel after being knocked (was set to stagger before getting in here)
            playerRigidBody.velocity = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interactable" && interactFocus == null)
        {
            interactFocus = collision.gameObject.GetComponent<Interactable>();
            interactFocus.OnFocused();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interactable" && interactFocus != null)
        {
            interactFocus.OnDefocused();
            interactFocus = null;
        }
    }
}
