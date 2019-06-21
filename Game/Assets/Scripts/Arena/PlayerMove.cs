using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : NetworkBehaviour {
	public float moveSpeed = 3;
	public float inCombatRange = 4;
	public float inCombatSpeedAdjust = 0.6f;
	public float lockCameraRangeAdjust = 3;
	public float attackingSpeedAdjust = 0.2f;
	public float turnSpeed = 3;
	[Space]
	public float moveSpeedMultiplier = 1;
	public float walkH = 0;
	public float walkV = 0;
	public bool canMove = true;
	public bool canRotateCamera = true;
	public bool isAttacking = false;

	Rigidbody rigidbody;
	//Animator animator;
	Robot robot;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		//animator = GetComponent<Animator>();
		robot = GetComponent<Robot>();
	}

	void Update() {
		float adjustSpeed = 1;
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			if (r == robot) {
				continue;
			}
			if (Vector3.Distance(robot.transform.position, r.transform.position) < inCombatRange) {
				robot.syncAnimator.SetBool("LB", true);
				adjustSpeed = inCombatSpeedAdjust;
			}
		}
		if (adjustSpeed == 1) {
			robot.syncAnimator.SetBool("LB", false);
		}
		if (isLocalPlayer && canMove) {
			// Move player
			//if (!isAttacking) {
			if (robot.lockCameraRobot) {
				float dist = Vector3.Distance(transform.position, robot.lockCameraRobot.transform.position) / lockCameraRangeAdjust;
				if (dist < 1) {
					adjustSpeed = dist;
				}
			}
			if (isAttacking) {
				adjustSpeed *= attackingSpeedAdjust;
			}
			//Debug.Log(moveSpeedMultiplier + " " + adjustSpeed);
			rigidbody.MovePosition(rigidbody.position + (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed * moveSpeedMultiplier * adjustSpeed);
			if (isAttacking) {
				walkH = 0;
				walkV = 0;
				//robot.SetFloat("WalkH", 0);
				//robot.SetFloat("WalkV", 0);
			} else {
				walkH = Input.GetAxis("Horizontal");
				walkV = Input.GetAxis("Vertical");
				//robot.SetFloat("WalkH", Input.GetAxis("Horizontal"));
				//robot.SetFloat("WalkV", Input.GetAxis("Vertical"));
			}
			/*} else {
				animator.SetFloat("WalkH", 0);
				animator.SetFloat("WalkV", 0);
			}*/
			// Rotate camera
			if (canRotateCamera) {
				rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Camera Horizontal") * turnSpeed));
			}
		} else {
			walkH = 0;
			walkV = 0;
			//robot.SetFloat("WalkH", 0);
			//robot.SetFloat("WalkV", 0);
		}
	}
}
