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
		player.CmdIncreaseComboScore();
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
			player.head.enabled = true;
		}
		if (enableHead) {
			player.head.enabled = true;
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
			if (player.leftHand.enabled) {
				player.CmdResetComboScore();
			}
			player.leftHand.enabled = false;
		}
		if (enableRightHand) {
			if (player.rightHand.enabled) {
				player.CmdResetComboScore();
			}
			player.rightHand.enabled = false;
		}
		if (enableLeftFoot) {
			if (player.leftFoot.enabled) {
				player.CmdResetComboScore();
			}
			player.leftFoot.enabled = false;
		}
		if (enableRightFoot) {
			if (player.rightFoot.enabled) {
				player.CmdResetComboScore();
			}
			player.rightFoot.enabled = false;
		}
		if (enableHead) {
			if (player.head.enabled) {
				player.CmdResetComboScore();
			}
			player.head.enabled = false;
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
