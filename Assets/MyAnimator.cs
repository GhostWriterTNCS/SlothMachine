using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class MyAnimator : NetworkBehaviour {
	public Camera camera;
	public float moveSpeed = 3;
	public Collider leftHand;
	public Collider rightHand;
	public SkinnedMeshRenderer body;
	public Material[] materials;
	static int materialIndex = 0;

	Animator animator;
	NetworkAnimator networkAnimator;

	void Start() {
		if (isLocalPlayer) {
			if (materialIndex >= materials.Length) {
				materialIndex = 0;
			}
			body.material = materials[materialIndex];
			materialIndex++;

			animator = GetComponent<Animator>();
			networkAnimator = GetComponent<NetworkAnimator>();
			leftHand.enabled = false;
			rightHand.enabled = false;
		} else {
			camera.enabled = false;
			camera.GetComponent<AudioListener>().enabled = false;
		}
	}

	void Update() {
		if (isLocalPlayer) {
			// Move player
			transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
			transform.position += Quaternion.AngleAxis(-90, transform.forward) * Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
			animator.SetFloat("WalkH", Input.GetAxis("Horizontal"));
			animator.SetFloat("WalkV", Input.GetAxis("Vertical"));

			// Actions
			if (Input.GetButtonDown("X")) {
				networkAnimator.SetTrigger("Punch");
			} else if (Input.GetButtonDown("Y")) {
				networkAnimator.SetTrigger("Boxing");
			}
		}
	}
}
