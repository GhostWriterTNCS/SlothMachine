using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Robot : NetworkBehaviour {
	[Header("Generic settings")]
	public GameObject hitEffect;
	public Material ionPlus;
	public Material ionMinus;
	public Material ionNull;
	public ParticleSystem ionParticle;
	public float comboDelay = 1;
	public float holdMinDuration = 0.76f;
	public float pushBackPower = 360;
	public float evadeDuration = 0.1f;
	public float evadeDistance = 0.3f;

	[Header("Generated settings")]
	public Player player;
	public Collider leftHand;
	public Collider rightHand;
	public Collider leftFoot;
	public Collider rightFoot;
	public Collider head;
	public Slider healthSlider;

	public GameObject handsParticle;

	[SyncVar]
	public int maxHealth = 100;
	[SyncVar(hook = "UpdateHealthSlider")]
	public float health;

	RobotModel robotModel;
	Animator animator;
	NetworkAnimator networkAnimator;
	PlayerCamera playerCamera;
	PlayerMove playerMove;
	Rigidbody rigidbody;
	float initialComboScore = 2;

	void Start() {
		player = GetComponentInParent<Player>();
		GameObject model = Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName), transform);
		robotModel = model.GetComponent<RobotModel>();
		if (!robotModel) {
			Debug.LogError("No robot model");
			return;
		}
		leftHand = robotModel.leftHand;
		leftHand.enabled = false;
		rightHand = robotModel.rightHand;
		rightHand.enabled = false;
		leftFoot = robotModel.leftFoot;
		leftFoot.enabled = false;
		rightFoot = robotModel.rightFoot;
		rightFoot.enabled = false;
		head = robotModel.head;
		head.enabled = false;

		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = robotModel.animatorController;
		animator.avatar = robotModel.avatar;
		networkAnimator = GetComponent<NetworkAnimator>();
		playerCamera = GetComponent<PlayerCamera>();
		playerMove = GetComponent<PlayerMove>();
		rigidbody = GetComponent<Rigidbody>();

		maxHealth = (int)(100 * (1 + (robotModel.health - 3) / 10f));
		CmdUpdateHealthValue(maxHealth);
		leftHand.enabled = false;
		rightHand.enabled = false;

		CmdResetComboScore();
	}

	float holdButton = 0;
	//[SyncVar]
	float holdDuration = 0;

	Vector3 evadeDirection;
	float evadeTime = 0;
	Transform lockCamera;
	void Update() {
		if (comboScoreDuration > 0) {
			comboScoreDuration -= Time.deltaTime;
		} else {
			if (comboScore > initialComboScore) {
				CmdResetComboScore();
			}
		}
		if (evadeTime > 0) {
			playerMove.canMove = false;
			rigidbody.MovePosition(rigidbody.position + (evadeDirection * evadeDistance));
			evadeTime -= Time.deltaTime;
		} else {
			playerMove.canMove = true;
			// Actions
			if (isLocalPlayer) {
				holdButton += Time.deltaTime;
				if (Input.GetButtonDown("A")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("A")) {
					SetTrigger("A");
				} else if (Input.GetButtonDown("B")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("B")) {
					if (Input.GetAxis("Horizontal") > 0.1) {
						evadeDirection = transform.right;
					} else if (Input.GetAxis("Horizontal") < -0.1) {
						evadeDirection = transform.right * -1;
					} else if (Input.GetAxis("Vertical") > 0.1) {
						evadeDirection = transform.forward;
					} else {
						evadeDirection = transform.forward * -1;
					}
					evadeTime = evadeDuration;
					SetTrigger("B");
				} else if (Input.GetButtonDown("X")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("X")) {
					holdDuration = holdButton;
					//Debug.Log(holdDuration + " " + (holdDuration >= holdMinDuration));
					SetTrigger("X");
				} else if (Input.GetButtonDown("Y")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("Y")) {
					holdDuration = holdButton;
					//Debug.Log(holdDuration + " " + (holdDuration >= holdMinDuration));
					SetTrigger("Y");
				}
				if (Input.GetButtonDown("LB")) {
					animator.SetBool("LB", true);
					playerMove.moveSpeedMultiplier = 0.55f + (robotModel.speed - 3) / 20f;
				} else if (Input.GetButtonUp("LB")) {
					animator.SetBool("LB", false);
					playerMove.moveSpeedMultiplier = 1 * (robotModel.speed - 3) / 12f;
				}
				if (Input.GetButtonDown("RS")) {
					if (lockCamera) {
						lockCamera = null;
					} else {
						RaycastHit hit;
						if (Physics.BoxCast(transform.position, new Vector3(3, 3, 3), transform.TransformDirection(Vector3.forward), out hit, Quaternion.identity, 30, 9)) {
							if (hit.transform.gameObject.GetComponent<Robot>()) {
								lockCamera = hit.transform;
							}
						}
					}
				}
				if (lockCamera) {
					transform.LookAt(lockCamera);
				}
				if (Input.GetButton("RB") && !Input.GetButton("RT")) {
					ionParticle.GetComponent<Renderer>().material = ionPlus;
				} else if (Input.GetButton("RT") && !Input.GetButton("RB")) {
					ionParticle.GetComponent<Renderer>().material = ionMinus;
				} else {
					ionParticle.GetComponent<Renderer>().material = ionNull;
				}
			}
		}
	}

	Dictionary<string, int> triggers = new Dictionary<string, int>();
	protected void SetTrigger(string trigger) {
		networkAnimator.SetTrigger(trigger);
		if (isServer) {
			networkAnimator.animator.ResetTrigger(trigger);
		}
		string triggerID = trigger;
		if (!triggers.ContainsKey(trigger)) {
			triggers.Add(trigger, 1);
		} else {
			triggers[trigger] += 1;
		}
		StartCoroutine(DelayCall(() => ResetTrigger(triggerID), comboDelay));
	}
	void ResetTrigger(string trigger) {
		triggers[trigger] -= 1;
		if (triggers[trigger] == 0) {
			networkAnimator.animator.ResetTrigger(trigger);
		}
	}

	[SyncVar]
	float comboScore;
	[SyncVar]
	float comboScoreDuration = 0;
	[Command]
	public void CmdIncreaseComboScore() {
		comboScore *= 1.5f;
		comboScoreDuration = 1;
		Debug.Log(player.name + " Multiply combo score: " + comboScore);
	}
	[Command]
	public void CmdResetComboScore() {
		comboScore = initialComboScore;
		Debug.Log(player.name + " Reset combo score: " + comboScore);
	}

	IEnumerator DelayCall(Action action, float delayTime) {
		yield return new WaitForSeconds(delayTime);
		action();
	}

	void UpdateHealthSlider(float value) {
		healthSlider.value = health / maxHealth;
	}
	[Command]
	public void CmdUpdateHealthValue(float newHealth) {
		health = newHealth;
		if (health <= 0) {
			CmdRespawn();
		}
	}
	public void UpdateHealth(float variation, bool isHeavy = false) {
		if (!animator.GetBool("LB") || isHeavy) {
			CmdUpdateHealthValue(health + variation);
		}
	}

	[Command]
	public void CmdGetHitted(GameObject hitterGO, Vector3 position) {
		Robot hitter = hitterGO.GetComponent<Robot>();
		//Debug.Log(player.name + " hitted by " + hitter.player.name + " | " + hitter.holdDuration);
		GameObject effect = Instantiate(hitter.hitEffect);
		effect.transform.position = position;
		effect.transform.localScale = Vector3.one;
		NetworkServer.Spawn(effect);
		animator.SetTrigger("Reaction");
		UpdateHealth(-5 * hitter.robotModel.attack / (float)robotModel.defense);
		hitter.player.scraps += 3;
		Debug.Log(hitter.player.name + " Current combo score: " + hitter.comboScore);
		hitter.player.score += (int)hitter.comboScore;
	}

	[Command]
	void CmdRespawn() {
		CmdUpdateHealthValue(maxHealth);
		var spawn = NetworkManager.singleton.GetStartPosition();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;
		rigidbody.velocity = Vector3.zero;
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

			leftParticle.transform.localPosition = leftHand.transform.localPosition;
			leftParticle.transform.localScale = Vector3.one;
			rightParticle.transform.localPosition = rightFoot.transform.localPosition;
			rightParticle.transform.localScale = Vector3.one;
		}
	}
}
