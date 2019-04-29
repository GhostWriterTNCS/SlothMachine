using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Robot : NetworkBehaviour {
	public float comboDelay = 1;
	float holdMinDuration = 0.76f;
	public float pushBackPower = 360;

	[SerializeField]
	public SphereCollider leftHand;
	[SerializeField]
	public SphereCollider rightHand;
	public SkinnedMeshRenderer body;
	public Material[] materials;
	public Slider healthSlider;

	[SerializeField]
	public GameObject handsParticle;

	[SyncVar]
	public int maxHealth = 100;
	[SyncVar]
	float health;

	Rigidbody rigidbody;
	Animator animator;
	NetworkAnimator networkAnimator;
	PlayerMove playerMove;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		networkAnimator = GetComponent<NetworkAnimator>();
		playerMove = GetComponent<PlayerMove>();

		UpdateHealthValue(maxHealth);
		leftHand.enabled = false;
		rightHand.enabled = false;
	}

	float holdButton = 0;
	[SyncVar]
	float holdDuration = 0;
	void Update() {
		// Actions
		if (isLocalPlayer) {
			holdButton += Time.deltaTime;
			if (Input.GetButtonDown("A")) {
				holdButton = 0;
			} else if (Input.GetButtonUp("A")) {
				holdDuration = holdButton;
				Debug.Log(holdDuration + " " + (holdDuration >= holdMinDuration));
				SetTrigger("A");
			} else if (Input.GetButtonDown("X")) {
				holdButton = 0;
			} else if (Input.GetButtonUp("X")) {
				holdDuration = holdButton;
				Debug.Log(holdDuration + " " + (holdDuration >= holdMinDuration));
				SetTrigger("X");
			} else if (Input.GetButtonDown("Y")) {
				holdButton = 0;
			} else if (Input.GetButtonUp("Y")) {
				SetTrigger("Y");
			}
			if (Input.GetButtonDown("LB")) {
				animator.SetBool("LB", true);
			} else if (Input.GetButtonUp("LB")) {
				animator.SetBool("LB", false);
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
		if (health <= 0) {
			CmdRespawn();
		}
	}
	public void UpdateHealth(float variation, bool isHeavy = false) {
		if (!animator.GetBool("LB") || isHeavy) {
			UpdateHealthValue(health + variation);
		}
	}

	public void GetHitted(Robot hitter) {
		Debug.Log(name + " hitted by " + hitter.name + " | " + hitter.holdDuration);
		animator.SetTrigger("Reaction");
		UpdateHealth(-5);
		/*if (hitter.holdDuration >= hitter.holdMinDuration) {
			rigidbody.AddForce(hitter.transform.forward * 1000, ForceMode.Impulse);
		}*/
	}

	[Command]
	void CmdRespawn() {
		UpdateHealthValue(maxHealth);
		var spawn = NetworkManager.singleton.GetStartPosition();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;
	}

	public void SetHandsParticle(GameObject particle) {
		Debug.Log("Set hands particle: " + particle.name);
		if (particle != handsParticle) {
			handsParticle = particle;
			for (int i = 0; i < leftHand.transform.childCount; i++) {
				Destroy(leftHand.transform.GetChild(i).gameObject);
			}
			GameObject leftParticle = Instantiate(particle);
			leftParticle.transform.SetParent(leftHand.transform);
			leftParticle.transform.localPosition = leftHand.center;
			leftParticle.transform.localScale = Vector3.one;

			for (int i = 0; i < rightHand.transform.childCount; i++) {
				Destroy(rightHand.transform.GetChild(i).gameObject);
			}
			GameObject rightParticle = Instantiate(particle);
			rightParticle.transform.SetParent(rightHand.transform);
			rightParticle.transform.localPosition = rightHand.center;
			rightParticle.transform.localScale = Vector3.one;
		}
	}
}
