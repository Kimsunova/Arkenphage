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
        if (currentState == EnemyState.dead)
            return;

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
                ChangeState(EnemyState.walk);
                myAnimator.SetBool("IsWalking", true);
                //myAnimator.SetBool("IsIdle", false);

            }
        }
        if (Vector3.Distance(target.position, transform.position) < attackRadius && (currentState != EnemyState.stagger && currentState != EnemyState.dead))
        {
            ChangeState(EnemyState.attack);
            myAnimator.SetBool("IsWalking", false);
            myRigidbody.velocity = Vector2.zero;
            myAnimator.SetTrigger("Attack");
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
        if (currentState == EnemyState.stagger)
            return; //don't want it to turn around when staggered

        //system permits finding movement direction of a NPC:
        if (previousPosition != transform.position)
        {
            currentMovementDirection = (previousPosition - transform.position).normalized;
            previousPosition = transform.position;
        }

        transform.localScale = new Vector2(Mathf.Sign(-currentMovementDirection.x), 1f);
    }
}
