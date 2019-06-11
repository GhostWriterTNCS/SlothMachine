using UnityEngine;

public class MyAnimation : StateMachineBehaviour {
	public bool enableLeftHand;
	public bool enableRightHand;
	public bool enableLeftFoot;
	public bool enableRightFoot;
	public bool enableHead;
	public bool breakGuard;
	public bool pushBack;

	Robot robot;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		robot = animator.GetComponent<Robot>();
		robot.CmdIncreaseComboScore();
		if (enableLeftHand) {
			robot.leftHand.enabled = true;
		}
		if (enableRightHand) {
			robot.rightHand.enabled = true;
		}
		if (enableLeftFoot) {
			robot.leftFoot.enabled = true;
		}
		if (enableRightFoot) {
			robot.rightFoot.enabled = true;
		}
		if (enableHead) {
			robot.head.enabled = true;
		}
		robot.breakGuard = breakGuard;
		robot.pushBack = pushBack;
		robot.GetComponent<PlayerMove>().isAttacking = true;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (enableLeftHand) {
			if (robot.leftHand.enabled) {
				robot.CmdResetComboScore();
			}
			robot.leftHand.enabled = false;
		}
		if (enableRightHand) {
			if (robot.rightHand.enabled) {
				robot.CmdResetComboScore();
			}
			robot.rightHand.enabled = false;
		}
		if (enableLeftFoot) {
			if (robot.leftFoot.enabled) {
				robot.CmdResetComboScore();
			}
			robot.leftFoot.enabled = false;
		}
		if (enableRightFoot) {
			if (robot.rightFoot.enabled) {
				robot.CmdResetComboScore();
			}
			robot.rightFoot.enabled = false;
		}
		if (enableHead) {
			if (robot.head.enabled) {
				robot.CmdResetComboScore();
			}
			robot.head.enabled = false;
		}

		foreach (BodyPartHitter h in robot.GetComponentsInChildren<BodyPartHitter>()) {
			h.hitters.Clear();
		}
		robot.GetComponent<PlayerMove>().isAttacking = false;
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
