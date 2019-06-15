using System.Collections;
using UnityEngine;

public class MyAnimation : StateMachineBehaviour {
	public bool enableLeftHand;
	public bool enableRightHand;
	public bool enableLeftFoot;
	public bool enableRightFoot;
	public bool enableHead;
	public bool breakGuard;
	public bool pushBack;
	public float hitDelay;

	Robot robot;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		robot = animator.GetComponent<Robot>();
		robot.CmdIncreaseComboScore();
		/*foreach (BodyPartHitter h in robot.GetComponentsInChildren<BodyPartHitter>()) {
			h.hitters.Clear();
		}*/
		//if (!robot.isServer || robot.isLocalPlayer) {
		//if (robot.isLocalPlayer) {
		robot.CmdEnableCollider(robot.gameObject, enableLeftHand, enableRightHand, enableLeftFoot, enableRightFoot, enableHead, hitDelay);
		/*	Debug.Log(robot.player.name + " is local player");
		} else {
			Debug.Log(robot.player.name + " is not local player");
		}*/
		robot.breakGuard = breakGuard;
		robot.pushBack = pushBack;
		robot.GetComponent<PlayerMove>().isAttacking = true;
	}

	IEnumerator EnableCollider() {
		if (hitDelay > 0) {
			yield return new WaitForSeconds(hitDelay);
		}
		if (enableLeftHand) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.leftHand, true);
			robot.leftHand.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableRightHand) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.rightHand, true);
			robot.rightHand.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableLeftFoot) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.leftFoot, true);
			robot.leftFoot.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableRightFoot) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.rightFoot, true);
			robot.rightFoot.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableHead) {
			robot.ActivateBodyPart(Robot.BodyPartCollider.head, true);
			robot.head.GetComponent<BodyPartHitter>().hitters.Clear();
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
