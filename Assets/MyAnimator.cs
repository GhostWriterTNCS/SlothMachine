using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class MyAnimator : NetworkBehaviour {
	Animator animator;
	public Camera camera;
	public float moveSpeed = 3;
	public Collider leftHand;
	public Collider rightHand;

	void Start() {
		if (isLocalPlayer) {
			animator = GetComponent<Animator>();
			leftHand.enabled = false;
			rightHand.enabled = false;
		} else {
			camera.enabled = false;
			camera.GetComponent<AudioListener>().enabled = false;
		}
	}

	void Update() {
		if (isLocalPlayer) {
			transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, transform.position.y, transform.position.z + Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed);
			animator.SetFloat("WalkH", Input.GetAxis("Horizontal"));
			animator.SetFloat("WalkV", Input.GetAxis("Vertical"));

			AnimationClip animationClip = new AnimationClip();
			if (animator.GetCurrentAnimatorClipInfo(0).Length > 0) {
				animationClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
			}
			if (Input.GetButtonDown("X")) {
				animator.SetTrigger("Punch");
			} else if (Input.GetButtonDown("Y")) {
				animator.SetTrigger("Boxing");
			}
		}
	}
}
