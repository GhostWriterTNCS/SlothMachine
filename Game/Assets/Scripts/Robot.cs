using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Robot : NetworkBehaviour {
	[SyncVar]
	public GameObject player;

	public float comboDelay = 1;
	float holdMinDuration = 0.76f;
	public float pushBackPower = 360;

	[SerializeField]
	public SphereCollider leftHand;
	[SerializeField]
	public SphereCollider rightHand;
	[SerializeField]
	public SphereCollider leftFoot;
	[SerializeField]
	public SphereCollider rightFoot;
	public Slider healthSlider;

	[SerializeField]
	public GameObject handsParticle;

	[SyncVar]
	public int maxHealth = 100;
	[SyncVar]
	float health;

	RobotModel robotModel;
	Animator animator;
	NetworkAnimator networkAnimator;
	PlayerMove playerMove;

	void Start() {
		//player = FindObjectOfType<Player>();
		if (!player) {
			Debug.Log("No player");
			Destroy(gameObject);
			return;
		}
		GameObject model = Instantiate(Resources.Load<GameObject>("Robots/" + player.GetComponent<Player>().robotModel + "/" + player.GetComponent<Player>().robotModel), transform);
		robotModel = model.GetComponent<RobotModel>();
		if (!robotModel) {
			Debug.LogError("No robot model");
			return;
		}
		leftHand = robotModel.leftHand;
		rightHand = robotModel.rightHand;
		leftFoot = robotModel.leftFoot;
		rightFoot = robotModel.rightFoot;

		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = robotModel.animatorController;
		animator.avatar = robotModel.avatar;
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
	}

	[Command]
	void CmdRespawn() {
		UpdateHealthValue(maxHealth);
		var spawn = NetworkManager.singleton.GetStartPosition();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;
	}

	List<GameObject> particles = new List<GameObject>();
	public void SetHandsParticle(GameObject particle, bool hands = true) {
		Debug.Log("Set hands particle: " + particle.name);
		if (particle != handsParticle) {
			handsParticle = particle;
			// Remove existing particles.
			for (int i = 0; i < particles.Count; i++) {
				Destroy(particles[i]);
			}
			particles.Clear();

			GameObject leftParticle = Instantiate(particle);
			GameObject rightParticle = Instantiate(particle);
			particles.Add(leftParticle);
			particles.Add(rightParticle);

			if (hands) {
				leftParticle.transform.SetParent(leftHand.transform);
				rightParticle.transform.SetParent(rightHand.transform);
			} else {
				leftParticle.transform.SetParent(leftFoot.transform);
				rightParticle.transform.SetParent(rightFoot.transform);
			}

			leftParticle.transform.localPosition = leftHand.center;
			leftParticle.transform.localScale = Vector3.one;
			rightParticle.transform.localPosition = rightFoot.center;
			rightParticle.transform.localScale = Vector3.one;
		}
	}
}
