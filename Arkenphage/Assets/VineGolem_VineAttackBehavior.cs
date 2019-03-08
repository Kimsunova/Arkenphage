using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineGolem_VineAttackBehavior : StateMachineBehaviour {

    [SerializeField] float speed = 35f;
    private float timer = 2f;
    private bool attacking = false;
    LineRenderer vineRenderer;
    Transform playerPos, savePlayerPos;
    float accum = 0f;
    Vector2 staticPos = Vector2.zero;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Idle", false);
        attacking = false;
        timer = 2f;
        accum = 0f;
        vineRenderer = animator.gameObject.GetComponent<LineRenderer>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        vineRenderer.enabled = true;
        savePlayerPos = playerPos;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Vector2 animatorPos = animator.transform.position;
        timer -= Time.deltaTime;
        
        if (attacking)
        {
            //if (!vineRenderer.enabled) vineRenderer.enabled = true;
            accum += Time.deltaTime;
            vineRenderer.positionCount = 2;
            vineRenderer.SetPosition(0, animatorPos);
            Vector2 vineEnd = animatorPos + (staticPos - animatorPos).normalized * accum * speed;
            vineRenderer.SetPosition(1, vineEnd);
            if (Vector2.Distance(vineRenderer.GetPosition(0), vineRenderer.GetPosition(1)) > Vector2.Distance(animatorPos, playerPos.position))
            {
                vineRenderer.enabled = false;
                animator.SetBool("Vine", false);
            }
        }
        else if (timer <= 0 && !attacking)
        {
            staticPos = playerPos.position;
            //vineRenderer.enabled = true;
            attacking = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        vineRenderer.SetPosition(0, animator.transform.position);
        vineRenderer.SetPosition(1, animator.transform.position);
        animator.SetBool("Idle", true);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
