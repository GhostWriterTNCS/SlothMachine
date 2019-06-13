﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Robot : NetworkBehaviour {
	[Header("Generic settings")]
	public GameObject hitEffect;
	public AudioClip hitSound;
	public GameObject fireParticle;
	//public AudioClip fireSound;
	public GameObject lightningParticle;
	//public AudioClip lightningSound;
	public GameObject iceParticle;
	//public AudioClip iceSound;
	public GameObject sonicParticle;
	//public AudioClip sonicSound;
	public SpriteRenderer minimapCursor;
	[Space]
	public Material ionPlus;
	public Material ionMinus;
	public Material ionNull;
	public ParticleSystem ionParticle;
	public ParticleSystem bossParticlePlus;
	public ParticleSystem bossParticleMinus;
	[Space]
	public float comboDelay = 1;
	public float holdMinDuration = 0.76f;
	public float pushBackPower = 360;
	public float evadeDuration = 0.1f;
	public float evadeDistance = 0.3f;
	public float evadeCooldown = 1;
	public AudioClip evadeSound;
	[Space]
	public GameObject keepOnRespawn;

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
	[Space]
	public Collider leftHand;
	public Collider rightHand;
	public Collider leftFoot;
	public Collider rightFoot;
	public Collider head;
	public Collider body;
	public bool breakGuard;
	public bool pushBack;
	[Space]
	public int leftHandCounter;
	public int rightHandCounter;
	public int leftFootCounter;
	public int rightFootCounter;
	public int headCounter;
	[Space]
	public Text nameText;
	public Slider healthSlider;
	public Image marker;
	[Space]
	public GameObject handsParticle;
	public GameObject feetParticle;
	public GameObject bodyParticle;
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
	[SyncVar]
	public int roundScore;
	[SyncVar]
	public bool paused;

	RobotModel robotModel;
	Animator animator;
	NetworkAnimator networkAnimator;
	PlayerCamera playerCamera;
	PlayerMove playerMove;
	Rigidbody rigidbody;
	float initialComboScore = 2;
	Text scrapsCounter;

	UpgradeWheel upgradeWheel;
	ArenaManager arenaManager;

	void Start() {
		StartCoroutine(SetupCoroutine());
	}
	IEnumerator SetupCoroutine() {
		paused = true;
		while (!playerGO) {
			yield return 0;
		}

		player = playerGO.GetComponent<Player>();
		player.robot = this;
		transform.SetParent(player.transform);
		arenaManager = FindObjectOfType<ArenaManager>();
		if (isLocalPlayer) {
			nameText.text = "";
			scrapsCounter = arenaManager.scrapsCounter;
			arenaManager.pauseMenu.robot = this;
		} else {
			nameText.text = player.name;
		}

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
		body = robotModel.body;
		if (!body.GetComponent<BodyPartTarget>()) {
			body.gameObject.AddComponent<BodyPartTarget>();
		}
		CmdMountUpgrades();

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

		marker = Instantiate(Resources.Load<GameObject>("Prefabs/Arena/Marker"), arenaManager.canvas.transform).GetComponent<Image>();
		marker.enabled = false;

		GameObject arenaBox = Instantiate(arenaManager.arenaBoxPrefab, arenaManager.leaderboard.transform);
		arenaBox.GetComponent<ArenaBox>().player = player;

		CmdCalculateBonus();
		CmdUpdateHealthValue(healthMax);

		CmdResetComboScore();
		upgradeWheel = FindObjectOfType<ArenaManager>().upgradeWheel;
		if (upgradeWheel)
			upgradeWheel.gameObject.SetActive(false);

		while (!arenaManager.arenaReady) {
			yield return 0;
		}

		if (player.roundWinner >= 2) {
			Debug.Log(player.name + " is Boss");
			Transform spawn = FindObjectOfType<BossArena>().bossSpawnPosition;
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
			transform.localScale = new Vector3(2, 2, 2);
			ionParticle.gameObject.SetActive(false);
			bossParticlePlus.gameObject.SetActive(true);
			bossParticleMinus.gameObject.SetActive(true);
		} else {
			Transform spawn = NetworkManager.singleton.GetStartPosition();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
		}
		rigidbody.velocity = Vector3.zero;

		minimapCursor.color = player.color;
	}

	[Command]
	public void CmdCalculateBonus() {
		healthBonus = 0;
		attackBonus = 0;
		defenseBonus = 0;
		speedBonus = 0;
		foreach (Pair upgrade in player.upgrades) {
			Upgrades.permanent[upgrade.value1][upgrade.value2].OnAdd(this);
		}
		CmdRefreshStats();
	}

	[Command]
	public void CmdRefreshStats() {
		healthMax = (robotModel.health + healthBonus) * 30;
		attack = robotModel.attack + attackBonus;
		defense = robotModel.defense + defenseBonus;
		if (robotModel.speed + speedBonus - 5 > 0) {
			speed = 1 + Mathf.Abs(robotModel.speed + speedBonus - 5) / 2;
		} else if (robotModel.speed + speedBonus - 5 < 0) {
			speed = 1 - Mathf.Abs(robotModel.speed + speedBonus - 5) / 4;
		} else {
			speed = 1;
		}
		if (player.roundWinner >= 2) {
			healthMax *= 2;
			defense *= 2;
		}
	}

	[Command]
	public void CmdMountUpgrades() {
		foreach (Pair p in player.upgrades) {
			GameObject prefab = Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/Upgrades/" + p.value1 + "_" + p.value2);
			if (prefab) {
				Prototype.NetworkLobby.LobbyManager.s_Singleton.spawnPrefabs.Add(prefab);
				MountUpgrade upgrade = Instantiate(prefab).GetComponent<MountUpgrade>();
				upgrade.type = p.value1;
				upgrade.ID = p.value2;
				upgrade.robotGO = gameObject;
				NetworkServer.Spawn(upgrade.gameObject);
			} else {
				Debug.LogError("Missing prefab: Prefabs/Robots/" + player.robotName + "/Upgrades/" + p.value1 + "_" + p.value2);
			}
		}
	}

	float holdButton = 0;
	//[SyncVar]
	float holdDuration = 0;

	Vector3 evadeDirection;
	float evadeTime = 0;
	float evadeCooldownTime = 0;
	public Robot lockCameraRobot;
	void Update() {
		if (paused) {
			playerMove.canMove = false;
			return;
		}
		if (player.isAgent && Input.GetKeyDown(KeyCode.G)) {
			GuardOn();
		}
		playerMove.canMove = true;
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
			if (isLocalPlayer) {
				if (evadeCooldownTime > 0) {
					evadeCooldownTime -= Time.deltaTime;
				}
				scrapsCounter.text = player.scraps.ToString();
				holdButton += Time.deltaTime;
				// Actions
				if (Input.GetButtonUp("Menu")) {
					arenaManager.pauseMenu.Pause();
				} else if (Input.GetButtonDown("A")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("A") && Input.GetAxis("Triggers") >= -0.01f) {
					SetTrigger("A");
				} else if (Input.GetButtonDown("B")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("B") && evadeCooldownTime <= 0) {
					if (Input.GetAxis("Horizontal") > 0.1) {
						evadeDirection = transform.right;
					} else if (Input.GetAxis("Horizontal") < -0.1) {
						evadeDirection = transform.right * -1;
					} else if (Input.GetAxis("Vertical") > 0.1) {
						evadeDirection = transform.forward;
					} else {
						evadeDirection = transform.forward * -1;
					}
					AudioManager.singleton.PlayClip(evadeSound);
					evadeTime = evadeDuration;
					evadeCooldownTime = evadeCooldown;
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
				} else if (Input.GetButtonUp("LB")) {
					GuardOff();
				}

				if (upgradeWheel)
					upgradeWheel.gameObject.SetActive(Input.GetAxis("Triggers") < -0.01f);
				playerMove.canRotateCamera = Input.GetAxis("Triggers") >= -0.01f;

				if (Input.GetButtonDown("RS")) {
					if (lockCameraRobot) {
						DisableLockCamera();
					} else {
						RaycastHit hit;
						if (Physics.BoxCast(transform.position, new Vector3(7, 7, 0.1f), transform.TransformDirection(Vector3.forward), out hit, Quaternion.identity, 20, 9)) {
							lockCameraRobot = hit.transform.gameObject.GetComponent<Robot>();
							// in the boss round, you can lock only the boss
							if (lockCameraRobot && lockCameraRobot.health > 0 && (!MatchManager.singleton.bossRound || lockCameraRobot.player.roundWinner >= 2)) {
								lockCameraRobot.marker.enabled = true;
							} else {
								lockCameraRobot = null;
							}
						}
					}
				}

				if (lockCameraRobot) {
					if (Vector3.Distance(transform.position, lockCameraRobot.transform.position) > 20) {
						DisableLockCamera();
					} else {
						transform.LookAt(lockCameraRobot.transform);

						// Final position of marker above GO in world space
						//Vector3 offsetPos = new Vector3(lockCameraRobot.transform.position.x, lockCameraRobot.transform.position.y + 1.5f, lockCameraRobot.transform.position.z);
						// Calculate *screen* position (note, not a canvas/recttransform position)
						Vector2 screenPoint = Camera.main.WorldToScreenPoint(lockCameraRobot.transform.position + new Vector3(0, 1, 0));
						// Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
						Vector2 canvasPos;
						RectTransformUtility.ScreenPointToLocalPointInRectangle(arenaManager.canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);
						// Set
						lockCameraRobot.GetComponent<Robot>().marker.transform.localPosition = canvasPos;
					}
				}

				// Ion particles
				if (Input.GetButton("RB") && Input.GetAxis("Triggers") <= 0.01f) {
					ionParticle.GetComponent<Renderer>().material = ionPlus;
				} else if (Input.GetAxis("Triggers") > 0.01f && !Input.GetButton("RB")) {
					ionParticle.GetComponent<Renderer>().material = ionMinus;
				} else {
					ionParticle.GetComponent<Renderer>().material = ionNull;
				}
			}
		}
	}

	public void DisableLockCamera() {
		lockCameraRobot.marker.enabled = false;
		lockCameraRobot = null;
		rigidbody.angularVelocity = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
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
		if (trigger != "B")
			playerMove.isAttacking = true;
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
		if (comboScore > 20) {
			comboScore = 20;
		}
		comboScoreDuration = 1;
	}
	[Command]
	public void CmdResetComboScore() {
		comboScore = initialComboScore;
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
		if (newHealth > healthMax) {
			newHealth = healthMax;
		}
		if (newHealth <= 0) {
			player.deathCount++;
		}
		health = newHealth;
	}
	public void UpdateHealth(float variation, bool isHeavy = false) {
		//if (!animator.GetBool("LB") || isHeavy) {
		float newHealth = health + variation;
		CmdUpdateHealthValue(newHealth);
		if (newHealth <= 0) {
			RpcRespawn();
		}
		//}
	}

	public enum BodyPartCollider {
		leftHand,
		rightHand,
		leftFoot,
		rightFoot,
		head
	}

	public void ActivateBodyPart(BodyPartCollider part, bool value) {
		switch (part) {
			case BodyPartCollider.leftHand:
				if (value) {
					leftHandCounter += 1;
				} else {
					leftHandCounter -= 1;
				}
				if (leftHandCounter > 0) {
					leftHand.enabled = true;
				} else {
					leftHand.enabled = false;
					playerMove.isAttacking = false;
				}
				break;
			case BodyPartCollider.rightHand:
				if (value) {
					rightHandCounter += 1;
				} else {
					rightHandCounter -= 1;
				}
				if (rightHandCounter > 0) {
					rightHand.enabled = true;
				} else {
					rightHand.enabled = false;
					playerMove.isAttacking = false;
				}
				break;
			case BodyPartCollider.leftFoot:
				if (value) {
					leftFootCounter += 1;
				} else {
					leftFootCounter -= 1;
				}
				if (leftFootCounter > 0) {
					leftFoot.enabled = true;
				} else {
					leftFoot.enabled = false;
					playerMove.isAttacking = false;
				}
				break;
			case BodyPartCollider.rightFoot:
				if (value) {
					rightFootCounter += 1;
				} else {
					rightFootCounter -= 1;
				}
				if (rightFootCounter > 0) {
					rightFoot.enabled = true;
				} else {
					rightFoot.enabled = false;
					playerMove.isAttacking = false;
				}
				break;
			case BodyPartCollider.head:
				if (value) {
					headCounter += 1;
				} else {
					headCounter -= 1;
				}
				if (headCounter > 0) {
					head.enabled = true;
				} else {
					head.enabled = false;
					playerMove.isAttacking = false;
				}
				break;
			default:
				Debug.LogError("Body part not recognized: " + part);
				break;
		}
	}

	[Command]
	public void CmdGetHitted(GameObject hitterGO, Vector3 position) {
		Robot hitter = hitterGO.GetComponent<Robot>();
		Debug.Log(hitter.name + " hits " + name + " " + hitter.pushBack);
		if (MatchManager.singleton.bossRound) {
			// avoid "friendly fire" in boss round
			if (player.roundWinner < 2 && hitter.player.roundWinner < 2) {
				return;
			}
		}
		if (!isGuardOn || hitter.breakGuard || hitter.player.roundWinner >= 2 ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) && !hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name)) {
			GameObject effect = Instantiate(hitter.hitEffect);
			AudioManager.singleton.PlayClip(hitSound);
			effect.transform.position = position;
			effect.transform.localScale = Vector3.one;
			NetworkServer.Spawn(effect);
			animator.SetTrigger("Reaction");
			float damage = hitter.attack * 2 * (1 - (defense * 5) / 100);
			if (health - damage <= 0) {
				hitter.UpdateHealth(hitter.healthMax / 3);
				if (hitter.lockCameraRobot) {
					hitter.DisableLockCamera();
				}
			}
			if (hitter.pushBack) {
				Debug.Log("Push");
				rigidbody.AddForce(hitter.transform.forward * 8.5f, ForceMode.Impulse);
			}
			UpdateHealth(-damage);
			hitter.player.scraps += 3;
			Debug.Log(hitter.player.name + " current combo score: " + hitter.comboScore);
			hitter.player.score += (int)hitter.comboScore;
			hitter.roundScore += (int)hitter.comboScore;
			GuardOff();
		}
	}

	float respawnWaiting;
	[ClientRpc]
	public void RpcRespawn() {
		StartCoroutine(RespawnCoroutine());
	}
	IEnumerator RespawnCoroutine() {
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).gameObject != keepOnRespawn) {
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		CmdSetPaused(gameObject, true);
		if (isLocalPlayer) {
			arenaManager.title.gameObject.SetActive(true);
		}
		if (MatchManager.singleton.bossRound) {
			arenaManager.title.text = arenaManager.youDefeated;
			yield break;
		}
		respawnWaiting = 3;
		while (respawnWaiting > 0) {
			respawnWaiting -= Time.deltaTime;
			if (isLocalPlayer) {
				arenaManager.title.text = arenaManager.respawnIn.Replace("#", ((int)respawnWaiting + 1).ToString());
			}
			yield return 0;
		}
		if (isLocalPlayer) {
			arenaManager.title.gameObject.SetActive(false);
		}
		CmdUpdateHealthValue(healthMax);
		CmdSetPaused(gameObject, false);
		Transform spawn = NetworkManager.singleton.GetStartPosition();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;
		rigidbody.velocity = Vector3.zero;
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(true);
		}
		if (player.roundWinner >= 2) {
			ionParticle.gameObject.SetActive(false);
		} else {
			bossParticlePlus.gameObject.SetActive(false);
			bossParticleMinus.gameObject.SetActive(false);
		}
	}

	[Command]
	public void CmdSetPaused(GameObject robot, bool value) {
		robot.GetComponent<Robot>().paused = value;
	}

	public enum BodyPart {
		Hands,
		Feet,
		Body
	}
	List<GameObject> handsParticles = new List<GameObject>();
	List<GameObject> feetParticles = new List<GameObject>();
	GameObject bodyParticles;
	public void SetUpgradeParticle(GameObject particle, BodyPart bodyPart) {
		Debug.Log("Set particle " + particle.name + " for " + bodyPart);
		Vector3 scale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
		switch (bodyPart) {
			case BodyPart.Hands:
				if (particle != handsParticle) {
					handsParticle = particle;
					// Remove existing particles.
					for (int i = 0; i < handsParticles.Count; i++) {
						Destroy(handsParticles[i]);
					}
					handsParticles.Clear();

					GameObject leftParticle = Instantiate(particle);
					GameObject rightParticle = Instantiate(particle);
					handsParticles.Add(leftParticle);
					handsParticles.Add(rightParticle);

					leftParticle.transform.SetParent(leftHand.transform);
					rightParticle.transform.SetParent(rightHand.transform);

					leftParticle.transform.localPosition = Vector3.zero;
					leftParticle.transform.localScale = scale;
					rightParticle.transform.localPosition = Vector3.zero;
					rightParticle.transform.localScale = scale;
				}
				break;
			case BodyPart.Feet:
				if (particle != feetParticle) {
					feetParticle = particle;
					// Remove existing particles.
					for (int i = 0; i < feetParticles.Count; i++) {
						Destroy(feetParticles[i]);
					}
					feetParticles.Clear();

					GameObject leftParticle = Instantiate(particle);
					GameObject rightParticle = Instantiate(particle);
					feetParticles.Add(leftParticle);
					feetParticles.Add(rightParticle);

					leftParticle.transform.SetParent(leftFoot.transform);
					rightParticle.transform.SetParent(rightFoot.transform);

					leftParticle.transform.localPosition = Vector3.zero;
					leftParticle.transform.localScale = scale;
					rightParticle.transform.localPosition = Vector3.zero;
					rightParticle.transform.localScale = scale;
				}
				break;
			default:
				if (particle != bodyParticle) {
					bodyParticle = particle;
					// Remove existing particles.
					Destroy(bodyParticles);

					bodyParticles = Instantiate(particle);

					bodyParticles.transform.SetParent(body.transform.GetChild(0));

					bodyParticles.transform.localPosition = Vector3.zero;
					bodyParticles.transform.localScale = scale * 1.5f;
				}
				break;
		}

	}
}
