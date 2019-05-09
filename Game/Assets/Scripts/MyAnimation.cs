using UnityEngine;

public class MyAnimation : StateMachineBehaviour {
	public bool enableLeftHand = false;
	public bool enableRightHand = false;
	public bool enableLeftFoot = false;
	public bool enableRightFoot = false;
    public bool enableHead = false;

	Robot player;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		player = animator.GetComponent<Robot>();
		if (enableLeftHand) {
			player.leftHand.enabled = true;
		}
		if (enableRightHand) {
			player.rightHand.enabled = true;
		}
		if (enableLeftFoot) {
			player.leftFoot.enabled = true;
		}
		if (enableRightFoot) {
			player.rightFoot.enabled = true;
		}
        if (enableHead) {                    //TODO modificare da rightFoot ad head
            player.rightFoot.enabled = true; //TODO modificare da rightFoot ad head 
        }
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (enableLeftHand) {
			player.leftHand.enabled = false;
		}
		if (enableRightHand) {
			player.rightHand.enabled = false;
		}
        if (enableLeftFoot) {
            player.leftFoot.enabled = false;
        }
        if (enableRightFoot) {
            player.rightFoot.enabled = false;
        }

        if (enableRightFoot) {                //TODO modificare da rightFoot ad head
            player.rightFoot.enabled = false; //TODO modificare da rightFoot ad head
        }

    }

	// OnStateMove is called right after Animator.OnAnimatorMove()
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that processes and affects root motion
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK()
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that sets up animation IK (inverse kinematics)
	//}
}
