using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle,
    dead,
    falling,
    grappling
}

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class Player : MonoBehaviour
{

    //Configurable Parameters
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpStrength = 5f;
    [SerializeField] float fallAnimationInitiateSpeed = 5f;
    [SerializeField] float fallMoveSpeed = 5f;



    //Player State
    bool jump = false;
    public PlayerState currentState;
    public FloatValue currentHealth;
    public Signal playerHealthSignal;
    public Signal playerHit;

    //public bool IsGrappling { get; set; }//should i use these properties like this?


    //Cached Component References
    Rigidbody2D playerRigidBody;
    Animator playerAnimator;
    CapsuleCollider2D playerBodyCollider;
    CircleCollider2D playerFeetCollider;
    PolygonCollider2D attackHitBox;

    //External variables
    public Interactable interactFocus;



    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<CircleCollider2D>();
        currentState = PlayerState.walk;
        attackHitBox = GetComponentInChildren<PolygonCollider2D>();

    }

    void Update()
    {
        if (currentState == PlayerState.interact || currentState == PlayerState.dead || currentState == PlayerState.stagger)
        {
            return;//this isn't working and player can still move around when state is dead
        }

        //new
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
        {
            StartCoroutine(AttackCo());
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle || currentState == PlayerState.falling)//need falling? should be able to move while falling?
        {
            //UpdateAnimationAndMove();
            Run();
        }

        if (Input.GetKeyDown(KeyCode.P) && interactFocus != null)
        {
            interactFocus.Interact();
        }





        DropDown();
        FlipSprite();
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

        if (currentState == PlayerState.dead)
        {
            playerAnimator.SetBool("Falling", false);
            return;
        }


        bool playFallingAnimation = playerRigidBody.velocity.y < -fallAnimationInitiateSpeed; //if player is moving

        if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")))
        {
            playFallingAnimation = false;
            //currentState = PlayerState.idle;//do this if also grappling hook is disabled
        }

        //if(!(currentState == PlayerState.grappling))
        //{
        //    //currentState = PlayerState.falling;//these seems wrong, it runs always that if not grappling then falling?
        //}
        //else
        //{
        //    playerIsFalling = true;//so always on falling animation if grappling (may need to chagne all this around later ifthere is a grappling animation)
        //}

        if(currentState == PlayerState.grappling)
        {
            playFallingAnimation = true; //always on falling animation if grappling
        }
        else
        {
            currentState = PlayerState.falling;
        }

        //if(currentState == PlayerState.falling)
        //{
        //    float horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        //    playerRigidBody.velocity += new Vector2(horizontalInput * fallMoveSpeed, 0f);
        //}

        playerAnimator.SetBool("Falling", playFallingAnimation);
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
        if (currentState == PlayerState.stagger || currentState == PlayerState.dead)
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
        currentState = PlayerState.attack;


        //playerAnimator.SetBool("attacking", true);//should be trigger and no need for coroutine?
        //currentState = PlayerState.attack;
        //yield return null;//waits one frame
        //playerAnimator.SetBool("attacking", false);//so it won't go back in here (why not just use a trigger type?)
        yield return new WaitForSeconds(.3f);//this still needed?
        if (currentState != PlayerState.interact)
        {
            currentState = PlayerState.walk;
        }
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
        currentState = PlayerState.dead;//i want to be able to set player state to dead here but it will prevent the death animation from happening sometimes, like maybe when falling?
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

            currentState = PlayerState.idle;//set back to idel after being knocked (was set to stagger before getting in here)
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
