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

	[Header("Bonus")]
	[SyncVar]
	public int healthBonus = 0;
	[SyncVar]
	public int attackBonus = 0;
	[SyncVar]
	public int defenseBonus = 0;
	[SyncVar]
	public int speedBonus = 0;

	[Header("Generated settings")]
	[SyncVar]
	public GameObject playerGO;
	public Player player;
	public Collider leftHand;
	public Collider rightHand;
	public Collider leftFoot;
	public Collider rightFoot;
	public Collider head;
	public Slider healthSlider;

	public GameObject handsParticle;
	public bool isGuardOn;

	[SyncVar(hook = "UpdateHealthSlider")]
	public float health;
	[SyncVar]
	public float healthMax;
	[SyncVar]
	public float attack;
	[SyncVar]
	public float defense;
	[SyncVar]
	public float speed;

	RobotModel robotModel;
	Animator animator;
	NetworkAnimator networkAnimator;
	PlayerCamera playerCamera;
	PlayerMove playerMove;
	Rigidbody rigidbody;
	float initialComboScore = 2;

	void Start() {
		StartCoroutine(SetupCoroutine());
	}
	IEnumerator SetupCoroutine() {
		while (!playerGO) {
			yield return new WaitForSeconds(0.05f);
		}

		player = playerGO.GetComponent<Player>();
		transform.SetParent(player.transform);
		GameObject model = Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName), transform);
		robotModel = model.GetComponent<RobotModel>();
		leftHand = robotModel.leftHand;
		leftHand.enabled = false;
		if (!leftHand.GetComponent<BodyPartHitter>()) {
			leftHand.gameObject.AddComponent<BodyPartHitter>();
		}
		rightHand = robotModel.rightHand;
		rightHand.enabled = false;
		if (!rightHand.GetComponent<BodyPartHitter>()) {
			rightHand.gameObject.AddComponent<BodyPartHitter>();
		}
		leftFoot = robotModel.leftFoot;
		leftFoot.enabled = false;
		if (!leftFoot.GetComponent<BodyPartHitter>()) {
			leftFoot.gameObject.AddComponent<BodyPartHitter>();
		}
		rightFoot = robotModel.rightFoot;
		rightFoot.enabled = false;
		if (!rightFoot.GetComponent<BodyPartHitter>()) {
			rightFoot.gameObject.AddComponent<BodyPartHitter>();
		}
		head = robotModel.head;
		head.enabled = false;
		if (!head.GetComponent<BodyPartHitter>()) {
			head.gameObject.AddComponent<BodyPartHitter>();
		}

		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = robotModel.animatorController;
		animator.avatar = robotModel.avatar;
		networkAnimator = GetComponent<NetworkAnimator>();
		playerCamera = GetComponent<PlayerCamera>();
		playerMove = GetComponent<PlayerMove>();
		rigidbody = GetComponent<Rigidbody>();

		leftHand.enabled = false;
		rightHand.enabled = false;
		isGuardOn = false;

		ArenaManager AM = FindObjectOfType<ArenaManager>();
		GameObject arenaBox = Instantiate(AM.arenaBoxPrefab, AM.leaderboard.transform);
		arenaBox.GetComponent<ArenaBox>().player = player;

		CmdCalculateBonus();
		CmdUpdateHealthValue(healthMax);

		CmdResetComboScore();
	}

	[Command]
	public void CmdCalculateBonus() {
		healthBonus = 0;
		attackBonus = 0;
		defenseBonus = 0;
		speedBonus = 0;
		foreach (Pair upgrade in player.upgrades) {
			Upgrades.list[upgrade.value1][upgrade.value2].OnAdd(this);
		}
		CmdRefreshStats();
	}

	[Command]
	public void CmdRefreshStats() {
		healthMax = 100 * (1 + (robotModel.health + healthBonus - 5) / 10f);
		attack = robotModel.attack + attackBonus;
		defense = robotModel.defense + defenseBonus;
		speed = 1 + (robotModel.speed + speedBonus - 5) / 12f;
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
					GuardOn();
					playerMove.moveSpeedMultiplier = speed * 0.55f;
				} else if (Input.GetButtonUp("LB")) {
					GuardOff();
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

	void GuardOn() {
		animator.SetBool("LB", true);
		isGuardOn = true;
		playerMove.moveSpeedMultiplier = speed * 0.55f;
	}
	void GuardOff() {
		animator.SetBool("LB", false);
		isGuardOn = false;
		playerMove.moveSpeedMultiplier = speed;
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
		//Debug.Log(player.name + " Multiply combo score: " + comboScore);
	}
	[Command]
	public void CmdResetComboScore() {
		comboScore = initialComboScore;
		//Debug.Log(player.name + " Reset combo score: " + comboScore);
	}

	IEnumerator DelayCall(Action action, float delayTime) {
		yield return new WaitForSeconds(delayTime);
		action();
	}

	void UpdateHealthSlider(float value) {
		healthSlider.value = value / healthMax;
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
		//Debug.Log(ionParticle.GetComponent<Renderer>().material.name + " ~ " + hitter.ionParticle.GetComponent<Renderer>().material.name);
		if (!isGuardOn ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) && !hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name)) {
			GameObject effect = Instantiate(hitter.hitEffect);
			effect.transform.position = position;
			effect.transform.localScale = Vector3.one;
			NetworkServer.Spawn(effect);
			animator.SetTrigger("Reaction");
			UpdateHealth(-5 * hitter.attack / defense);
			hitter.player.scraps += 3;
			Debug.Log(hitter.player.name + " Current combo score: " + hitter.comboScore);
			hitter.player.score += (int)hitter.comboScore;
			GuardOff();
		}
	}

	[Command]
	void CmdRespawn() {
		CmdUpdateHealthValue(healthMax);
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
