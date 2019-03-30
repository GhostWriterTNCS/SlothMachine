using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class MyAnimator : NetworkBehaviour {
	public Camera playerCamera;
	public float moveSpeed = 3;
	public float turnSpeed = 3;
	public Collider leftHand;
	public Collider rightHand;
	public SkinnedMeshRenderer body;
	public Material[] materials;
	static int materialIndex = 0;

	Vector3 cameraOffset;
	Quaternion cameraRotation;
	Animator animator;
	NetworkAnimator networkAnimator;

	void Start() {
		if (isLocalPlayer) {
			if (materialIndex >= materials.Length) {
				materialIndex = 0;
			}
			body.material = materials[materialIndex];
			materialIndex++;

			cameraOffset = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y, playerCamera.transform.localPosition.z);
			cameraRotation = playerCamera.transform.localRotation;
			Debug.Log(cameraRotation);
			animator = GetComponent<Animator>();
			networkAnimator = GetComponent<NetworkAnimator>();
			leftHand.enabled = false;
			rightHand.enabled = false;
		} else {
			playerCamera.enabled = false;
			playerCamera.GetComponent<AudioListener>().enabled = false;
		}
	}

	void Update() {
		if (isLocalPlayer) {
			// Move player and rotate camera
			transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
			transform.position += Quaternion.AngleAxis(-90, transform.forward) * Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
			transform.localRotation *= Quaternion.Euler(Vector3.up * Input.GetAxis("Camera Horizontal") * turnSpeed);
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
