using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody)), NetworkSettings(channel = 2)]
public class PlayerMove : NetworkBehaviour {
	public float moveSpeed = 3;
	public float inCombatRange = 4;
	public float inCombatSpeedAdjust = 0.6f;
	public float lockCameraRangeAdjust = 3;
	public float attackingSpeedAdjust = 0.2f;
	public float turnSpeed = 3;
	[Space]
	public float moveSpeedMultiplier = 1;
	[SyncVar]
	public float walkH = 0;
	[SyncVar]
	public float walkV = 0;
	public bool canMove = true;
	public bool canRotateCamera = true;
	public bool isAttacking = false;

	Rigidbody rigidbody;
	Robot robot;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		robot = GetComponent<Robot>();
	}

	void Update() {
		float adjustSpeed = 1, h = 0, v = 0;
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			if (r == robot) {
				continue;
			}
			if (Vector3.Distance(robot.transform.position, r.transform.position) < inCombatRange) {
				robot.animator.SetBool("LB", true);
				adjustSpeed = inCombatSpeedAdjust;
			}
		}
		if (adjustSpeed == 1) {
			robot.animator.SetBool("LB", false);
		}
		if (isLocalPlayer) {
			if (canMove) {
				// Move player
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
				if (!isAttacking) {
					h = Input.GetAxis("Horizontal");
					v = Input.GetAxis("Vertical");
				}

				// Rotate camera
				if (canRotateCamera) {
					rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Camera Horizontal") * turnSpeed));
				}
			}
			if (h != walkH) {
				CmdSetWalkH(h);
			}
			if (v != walkV) {
				CmdSetWalkV(v);
			}
		}
	}

	[Command]
	void CmdSetWalkH(float value) {
		walkH = value;
	}

	[Command]
	void CmdSetWalkV(float value) {
		walkV = value;
	}
}
