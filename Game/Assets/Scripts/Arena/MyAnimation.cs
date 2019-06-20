using System.Collections;
using UnityEngine;

public class MyAnimation : StateMachineBehaviour {
	public bool enableLeftHand;
	public bool enableRightHand;
	public bool enableLeftFoot;
	public bool enableRightFoot;
	public bool enableHead;
	[Space]
	public bool breakGuard;
	public bool pushBack;
	public float hitDelay;
	public bool resetDirection;

	Robot robot;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		robot = animator.GetComponentInParent<Robot>();
		robot.CmdIncreaseComboScore();
		robot.CmdEnableCollider(enableLeftHand, enableRightHand, enableLeftFoot, enableRightFoot, enableHead, breakGuard, pushBack, hitDelay);
		robot.GetComponentInParent<PlayerMove>().isAttacking = true;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		robot.CmdDisableCollider(enableLeftHand, enableRightHand, enableLeftFoot, enableRightFoot, enableHead);
		/*if (enableLeftHand) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.leftHand, false);
		}
		if (enableRightHand) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.rightHand, false);
		}
		if (enableLeftFoot) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.leftFoot, false);
		}
		if (enableRightFoot) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.rightFoot, false);
		}
		if (enableHead) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.head, false);
		}

		/*foreach (BodyPartHitter h in robot.GetComponentsInChildren<BodyPartHitter>()) {
			h.hitters.Clear();
		}*/
		robot.GetComponentInParent<PlayerMove>().isAttacking = false;
		if (resetDirection) {
			robot.robotModel.transform.localRotation = Quaternion.identity;
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
