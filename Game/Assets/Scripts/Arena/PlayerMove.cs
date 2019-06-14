using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : NetworkBehaviour {
	public float moveSpeed = 3;
	public float moveSpeedMultiplier = 1;
	public float moveSpeedAdjust = 3;
	public float turnSpeed = 3;
	public bool canMove = true;
	public bool canRotateCamera = true;
	public bool isAttacking = false;

	Rigidbody rigidbody;
	Animator animator;
	Robot robot;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		robot = GetComponent<Robot>();
	}

	void Update() {
		if (isLocalPlayer && canMove) {
			// Move player
			//if (!isAttacking) {
				float adjustSpeed = 1;
				if (robot.lockCameraRobot) {
					float dist = Vector3.Distance(transform.position, robot.lockCameraRobot.transform.position) / moveSpeedAdjust;
					if (dist < 1) {
						adjustSpeed = dist;
					}
				}
                if(isAttacking)
            {
                adjustSpeed *= 0.2f;
            }
				rigidbody.MovePosition(rigidbody.position + (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed * moveSpeedMultiplier * adjustSpeed);
            if (isAttacking)
            {
                animator.SetFloat("WalkH", 0);
                animator.SetFloat("WalkV", 0);
            }else
            {
                animator.SetFloat("WalkH", Input.GetAxis("Horizontal"));
				animator.SetFloat("WalkV", Input.GetAxis("Vertical"));
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
			animator.SetFloat("WalkH", 0);
			animator.SetFloat("WalkV", 0);
		}
	}
}
