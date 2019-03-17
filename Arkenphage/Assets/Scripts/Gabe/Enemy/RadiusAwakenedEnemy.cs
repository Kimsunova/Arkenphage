using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusAwakenedEnemy : Enemy
{

    private Rigidbody2D myRigidbody;
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform homePosition;
    //[SerializeField] protected Animator myAnimator;
    private Vector3 previousPosition;
    private Vector3 currentMovementDirection;
    private float previousDirectionScale;
    private float currentDirectionScale;

    // Use this for initialization
    void Start()
    {
        currentState = EnemyState.idle;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState == EnemyState.dead)
            return;

        CheckDistance();
    }

    private void Update()
    {
        //if (currentState == EnemyState.dead)
        //    return;

        FlipSprite();
    }

    void CheckDistance()
    {
        if (Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attackRadius)
        {
            if ((currentState == EnemyState.idle || currentState == EnemyState.walk) && (currentState != EnemyState.stagger && currentState != EnemyState.dead))
            {

                Vector3 targetGroundPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
                Vector3 temp = Vector3.MoveTowards(transform.position, targetGroundPosition, moveSpeed * Time.deltaTime);

                myRigidbody.MovePosition(temp);
                //ChangeState(EnemyState.walk);//this doesn't need to be set here I guess?
                myAnimator.SetBool("IsWalking", true);
                //myAnimator.SetBool("IsIdle", false);

            }
        }
        if (Vector3.Distance(target.position, transform.position) < attackRadius && (currentState != EnemyState.stagger && currentState != EnemyState.dead))
        {
            ChangeState(EnemyState.attack);
            myAnimator.SetBool("IsWalking", false);
            myRigidbody.velocity = Vector2.zero;
            myAnimator.SetTrigger("Attack");//because this is a trigger cannot be interrupted by death?
            ChangeState(EnemyState.idle);//should there be a wait for seconds before this?
        }
        if (Vector3.Distance(target.position, transform.position) > chaseRadius)
        {
            myAnimator.SetBool("IsWalking", false);
            ChangeState(EnemyState.idle);

            //myAnimator.SetBool("IsIdle", true);

        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    private void FlipSprite()
    {
        if (currentState == EnemyState.stagger || currentState == EnemyState.dead)
            return; //don't want it to turn around when staggered or dead, still turning when dead though

        previousDirectionScale = currentDirectionScale;

        //system permits finding movement direction of a NPC:
        if (previousPosition != transform.position)
        {
            currentMovementDirection = (previousPosition - transform.position).normalized;
            previousPosition = transform.position;
        }

        currentDirectionScale = Mathf.Sign(-currentMovementDirection.x);

        if(currentDirectionScale != previousDirectionScale)
        {
            transform.localScale = new Vector2(currentDirectionScale, 1f);
        }

    }
}
