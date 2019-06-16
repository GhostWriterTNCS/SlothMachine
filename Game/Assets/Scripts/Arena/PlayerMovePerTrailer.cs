using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovePerTrailer : MonoBehaviour {
	public float moveSpeed = 3;
	public float moveSpeedMultiplier = 1;
	public float moveSpeedAdjust = 3;
	public float turnSpeed = 3;
	public bool canMove = true;
	public bool canRotateCamera = true;
	public bool isAttacking = false;
	public float walkH = 0;
	public float walkV = 0;

	Rigidbody rigidbody;
	//Animator animator;
	Robot robot;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		//animator = GetComponent<Animator>();
		robot = GetComponent<Robot>();
	}

	void Update() {
		if (canMove) {
			// Move player
			//if (!isAttacking) {
			float adjustSpeed = 1;
            /*if (robot.lockCameraRobot) {
				float dist = Vector3.Distance(transform.position, robot.lockCameraRobot.transform.position) / moveSpeedAdjust;
				if (dist < 1) {
					adjustSpeed = dist;
				}
			}*/
            if (Input.GetKey("w"))
            {
                GetComponent<Animator>().SetFloat("WalkV", 1);
                rigidbody.MovePosition(rigidbody.position + (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed * moveSpeedMultiplier * adjustSpeed);
            }
            else
            {
                GetComponent<Animator>().SetFloat("WalkV", 0);
                if (isAttacking)
                {
                    adjustSpeed *= 0.2f;
                }
                rigidbody.MovePosition(rigidbody.position + (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed * moveSpeedMultiplier * adjustSpeed);
                if (isAttacking)
                {
                    walkH = 0;
                    walkV = 0;
                    //robot.SetFloat("WalkH", 0);
                    //robot.SetFloat("WalkV", 0);
                }
                else
                {
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
                if (canRotateCamera)
                {
                    rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Camera Horizontal") * turnSpeed));
                }
            }

			
		} else {
			walkH = 0;
			walkV = 0;
			//robot.SetFloat("WalkH", 0);
			//robot.SetFloat("WalkV", 0);
		}
	}
}
