using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class MyAnimator : NetworkBehaviour {
	public Camera playerCamera;
	public float moveSpeed = 3;
	public float turnSpeed = 3;
	public float comboDelay = 1;
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
		if (!isLocalPlayer) {
			playerCamera.enabled = false;
			playerCamera.GetComponent<AudioListener>().enabled = false;
		}
		cameraOffset = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y, playerCamera.transform.localPosition.z);
		cameraRotation = playerCamera.transform.localRotation;
		animator = GetComponent<Animator>();
		networkAnimator = GetComponent<NetworkAnimator>();

		UpdateHealthValue(maxHealth);
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
				SetTrigger("X");
			} else if (Input.GetButtonDown("Y")) {
				SetTrigger("Y");
			}
			if (Input.GetButtonDown("LB")) {
				animator.SetBool("LB", true);
			} else if (Input.GetButtonUp("LB")) {
				animator.SetBool("LB", false);
			}

			// Adjust health sliders orientation
			foreach (MyAnimator a in FindObjectsOfType<MyAnimator>()) {
				a.GetComponentInChildren<Canvas>().transform.LookAt(playerCamera.transform);
			}
		}
	}

	protected void SetTrigger(string trigger) {
		networkAnimator.SetTrigger(trigger);
		if (isServer) {
			networkAnimator.animator.ResetTrigger(trigger);
		}
		string triggerID = trigger;
		StartCoroutine(DelayCall(() => networkAnimator.animator.ResetTrigger(triggerID), comboDelay));
	}

	IEnumerator DelayCall(Action action, float delayTime) {
		yield return new WaitForSeconds(delayTime);
		action();
	}

	public void UpdateHealthValue(float newHealth) {
		Debug.Log(health + " -> " + newHealth);
		health = newHealth;
		healthSlider.value = health / maxHealth;
	}
	public void UpdateHealth(float variation, bool isHeavy = false) {
		if (!animator.GetBool("LB") || isHeavy) {
			UpdateHealthValue(health + variation);
		}
	}
}