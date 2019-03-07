using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineGolem_RootGrabBehavior : StateMachineBehaviour {
    [SerializeField] GameObject root;
    private float timer = 2f;
    private bool attacking = false;
    private Transform playerPos;
    private Animator rootAnimator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (root == null)
        {
            root = animator.gameObject.GetComponent<RootLinker>().root;
            rootAnimator = root.GetComponentInChildren<Animator>();
        }
        animator.SetBool("Idle", false);
        attacking = false;
        timer = 2f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        timer -= Time.deltaTime;
        if (timer <= 0 && attacking == false)
        {
            RootGrab(animator);
            attacking = true;
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Idle", true);
    }

    void RootGrab(Animator animator)
    {
        if (root != null)
        {
            root.transform.position = new Vector2(playerPos.position.x, animator.gameObject.transform.position.y - 10f);
            rootAnimator.SetTrigger("Grab");
        }
        timer = 2f;
        attacking = false;
        animator.SetBool("Root", false);
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
