using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class MyAnimator : NetworkBehaviour {
	public Camera playerCamera;
	public float moveSpeed = 3;
	public float turnSpeed = 3;
	[SerializeField]
	public Collider leftHand;
	[SerializeField]
	public Collider rightHand;
	public SkinnedMeshRenderer body;
	public Material[] materials;
	public Slider healthSlider;

	//From server to client: keeps the maximum health of every tank in game
	[SyncVar]
	public int maxHealth = 100;

	//From server to client: keeps the current health of every tank. If the value is updated, "UpdateHealth" is executed on clients
	[SyncVar]
	float health;

	Vector3 cameraOffset;
	Quaternion cameraRotation;
	Animator animator;
	NetworkAnimator networkAnimator;

	void Start() {
		if (isLocalPlayer) {
			/*if (materialIndex >= materials.Length) {
				materialIndex = 0;
			}
			body.material = materials[materialIndex];
			materialIndex++;*/

			cameraOffset = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y, playerCamera.transform.localPosition.z);
			cameraRotation = playerCamera.transform.localRotation;
			animator = GetComponent<Animator>();
			networkAnimator = GetComponent<NetworkAnimator>();

			UpdateHealthValue(maxHealth);
		} else {
			playerCamera.enabled = false;
			playerCamera.GetComponent<AudioListener>().enabled = false;
		}
		leftHand.enabled = false;
		rightHand.enabled = false;
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
				networkAnimator.SetTrigger("X");
				if (isServer) {
					networkAnimator.animator.ResetTrigger("X");
				}
			} else if (Input.GetButtonDown("Y")) {
				networkAnimator.SetTrigger("Y");
				if (isServer) {
					networkAnimator.animator.ResetTrigger("Y");
				}
			}
		}
	}

	protected void SetTrigger(string trigger) {
		networkAnimator.SetTrigger(trigger);
		if (isServer) {
			networkAnimator.animator.ResetTrigger(trigger);
		}
	}

	public void UpdateHealthValue(float newHealth) {
		health = newHealth;
		healthSlider.value = health / maxHealth;
	}
	public void UpdateHealth(float variation) {
		UpdateHealthValue(health + variation);
	}
}