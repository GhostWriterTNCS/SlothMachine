﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator)), NetworkSettings(channel = 3)]
public class Robot : NetworkBehaviour {
	[Header("Generic settings")]
	public GameObject hitEffect;
	public GameObject fireParticle;
	public GameObject lightningParticle;
	public GameObject iceParticle;
	public GameObject sonicParticle;
	public SpriteRenderer minimapCursor;
	[Header("Ion")]
	public Material ionPlus;
	public Material ionMinus;
	public Material ionNull;
	public ParticleSystem ionParticle;
	public ParticleSystem bossParticlePlus;
	public ParticleSystem bossParticleMinus;
	[Header("Evade")]
	public float evadeDuration = 0.1f;
	public float evadeDistance = 0.3f;
	public float evadeCooldown = 1;
	[Header("Others")]
	public float comboDelay = 1;
	public float holdMinDuration = 0.76f;
	public float pushBackPower = 360;
	public float baseMass = 50;
	public GameObject keepOnRespawn;
	public AudioClip[] clips;
	public enum AudioClips {
		Dash,
		Destroyed,
		Hit,
		Fire,
		Ice,
		Lightning,
		Sonic,
		Repair
	}

	[Header("Bonus")]
	[SyncVar]
	public byte healthBonus = 0;
	[SyncVar]
	public byte attackBonus = 0;
	[SyncVar]
	public byte defenseBonus = 0;
	[SyncVar]
	public byte speedBonus = 0;

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
	public int[] upgrades;
	//public bool isGuardOn;

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
	public short roundScore;
	[SyncVar]
	public bool paused;

	public RobotModel robotModel;
	public Animator animator;
	PlayerCamera playerCamera;
	public PlayerMove playerMove;
	Rigidbody rigidbody;
	//AudioSource audioSource;
	short initialComboScore = 2;
	float evadeDelay;

	UpgradeWheel upgradeWheel;
	ArenaManager arenaManager;
	SyncTransform syncTransform;
	public SyncAnimator syncAnimator;

	void Start() {
		upgrades = new int[4];
		transform.position = Vector3.one * 5;
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
		while (!FindObjectOfType<ArenaManager>()) {
			yield return 0;
		}
		arenaManager = FindObjectOfType<ArenaManager>();
		syncTransform = GetComponent<SyncTransform>();
		syncAnimator = GetComponent<SyncAnimator>();
		if (isLocalPlayer) {
			nameText.text = "";
			arenaManager.pauseMenu.robot = this;
		} else {
			nameText.text = player.name;
			if (player.isAgent) {
				syncTransform.EnableSetValues(true);
			}
		}
		roundScore = 0;

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
		robotModel.shield.SetActive(false);
		CmdMountUpgrades();

		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = robotModel.animatorController;
		animator.avatar = robotModel.avatar;
		playerCamera = GetComponent<PlayerCamera>();
		playerMove = GetComponent<PlayerMove>();
		rigidbody = GetComponent<Rigidbody>();
		//audioSource = GetComponent<AudioSource>();

		leftHand.enabled = false;
		rightHand.enabled = false;
		robotModel.shield.SetActive(false);

		marker = Instantiate(Resources.Load<GameObject>("Prefabs/Arena/Marker"), arenaManager.canvas.transform).GetComponent<Image>();
		marker.enabled = false;

		GameObject arenaBox = Instantiate(arenaManager.arenaBoxPrefab, arenaManager.leaderboard.transform);
		arenaBox.GetComponent<ArenaBox>().player = player;

		CmdResetComboScore();
		upgradeWheel = arenaManager.upgradeWheel;
		if (upgradeWheel)
			upgradeWheel.gameObject.SetActive(false);

		minimapCursor.color = player.color;
		if (isLocalPlayer) {
			minimapCursor.transform.localScale *= 1.5f;
		}
		evadeDelay = robotModel.evadeDelay;

		body.enabled = false;
		while (!arenaManager.arenaReady) {
			yield return 0;
		}
		CmdCalculateBonus();

		//Transform spawn = NetworkManager.singleton.GetStartPosition();
		if (player.roundWinner >= 2) {
			Debug.Log(player.name + " is Boss");
			Transform spawn = FindObjectOfType<BossArena>().bossSpawnPosition;
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
			rigidbody.velocity = Vector3.zero;
			syncTransform.CmdSetPosition(transform.position);
			syncTransform.CmdSetRotation(transform.rotation);

			transform.localScale = new Vector3(2, 2, 2);
			ionParticle.gameObject.SetActive(false);
			bossParticlePlus.gameObject.SetActive(true);
			bossParticleMinus.gameObject.SetActive(true);
		} else {
			//CmdSpawn();
			int bossID = 0;
			if (MatchManager.singleton.bossRound) {
				foreach (Player p in FindObjectsOfType<Player>()) {
					if (p.roundWinner >= 2) {
						bossID = p.playerID;
						break;
					}
				}
			}
			Transform spawn;
			if (bossID > 0 && player.playerID > bossID) {
				spawn = FindObjectsOfType<SpawnPoint>()[player.playerID - 2].transform;
			} else {
				spawn = FindObjectsOfType<SpawnPoint>()[player.playerID - 1].transform;
			}
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
			rigidbody.velocity = Vector3.zero;
		}
		body.enabled = true;

		if (isLocalPlayer) {
			arenaManager.upgradeWheel.player = player;
		}
		while (healthMax == 0) {
			yield return 0;
		}
		CmdUpdateHealthValue(healthMax);
	}

	[Command]
	public void CmdCalculateBonus() {
		healthBonus = 0;
		attackBonus = 0;
		defenseBonus = 0;
		speedBonus = 0;
		foreach (Pair upgrade in player.upgrades) {
			if (upgrade) {
				Upgrades.permanent[upgrade.value1][upgrade.value2].OnAdd(this);
			}
		}
		CmdRefreshStats();
	}

	[Command]
	public void CmdRefreshStats() {
		healthMax = (robotModel.health + healthBonus) * 30;
		attack = robotModel.attack + attackBonus;
		defense = robotModel.defense + defenseBonus;
		speed = 1 + (robotModel.speed + speedBonus - 5) / 16f;
		Debug.Log(robotModel.name + " speed is " + speed);
		rigidbody.mass = baseMass / speed;
		if (player.roundWinner >= 2) {
			healthMax *= 2;
			attack = (byte)(attack * 1.25f);
			defense *= 2;
		}
		if (defense > 14) {
			defense = 14;
		}
		Debug.Log("Stats updated");
	}

	[Command]
	public void CmdMountUpgrades() {
		Debug.Log(player.name + " has " + player.upgrades.Length + " upgrades.");
		MountUpgrades();
		RpcMountUpgrades();
	}
	[ClientRpc]
	void RpcMountUpgrades() {
		MountUpgrades();
	}
	void MountUpgrades() {
		foreach (Pair p in player.upgrades) {
			if (p) {
				Debug.Log(player.name + " mounts " + p.value1 + "_" + p.value2);
				GameObject prefab = Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/Upgrades/" + p.value1 + "_" + p.value2);
				if (prefab) {
					GameObject upgradeGO = Instantiate(prefab);
					MountUpgrade upgrade = upgradeGO.GetComponent<MountUpgrade>();
					if (!upgrade) {
						upgrade = upgradeGO.AddComponent<MountUpgrade>();
					}
					upgrade.type = (byte)p.value1;
					upgrade.ID = (byte)p.value2;
					upgrade.robotGO = gameObject;
				} else {
					Debug.LogWarning("Missing prefab: Prefabs/Robots/" + player.robotName + "/Upgrades/" + p.value1 + "_" + p.value2);
				}
			}
		}
	}

	float holdButton = 0;
	//float holdDuration = 0;

	Vector3 evadeDirection;
	float evadeDelayTime = 0;
	float evadeTime = 0;
	float evadeCooldownTime = 0;
	float lockCameraMaxDistance = 20;
	RaycastHit hit;
	bool upgradeWheelActive;
	public Robot lockCameraRobot;
	void Update() {
		if (!player || !playerMove) {
			return;
		}
		if (paused) {
			playerMove.canMove = false;
			return;
		}
		if (player.isAgent && Input.GetKeyDown(KeyCode.G)) {
			SetShield(true);
		}
		playerMove.canMove = true;
		if (comboScoreDuration > 0) {
			comboScoreDuration -= Time.deltaTime;
		} else {
			if (comboScore > initialComboScore) {
				CmdResetComboScore();
			}
		}
		if (evadeDelayTime > 0) {
			playerMove.canMove = false;
			evadeDelayTime -= Time.deltaTime;
		} else if (evadeTime > 0) {
			playerMove.canMove = false;
			rigidbody.MovePosition(rigidbody.position + (evadeDirection * evadeDistance));
			evadeTime -= Time.deltaTime;
		} else {
			if (isLocalPlayer) {
				if (evadeCooldownTime > 0) {
					evadeCooldownTime -= Time.deltaTime;
					arenaManager.evadeCooldown.fillAmount = 1 - (evadeCooldownTime / evadeCooldown);
				}
				arenaManager.bottomBar.SetActive(true);
				arenaManager.scrapsCounter.text = player.scraps.ToString();
				holdButton += Time.deltaTime;
				// Actions
				if (Input.GetButtonUp("Menu")) {
					arenaManager.pauseMenu.Pause();
				} else if (Input.GetButtonDown("A")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("A") && Input.GetAxis("Triggers") >= -0.01f) {
					syncAnimator.SetTrigger("A");
				} else if (Input.GetButtonDown("B")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("B") && evadeCooldownTime <= 0) {
					evadeDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
					if (evadeDirection.magnitude < 0.2f) {
						evadeDirection = transform.forward * -1;
					}
					robotModel.transform.rotation = Quaternion.LookRotation(evadeDirection);
					CmdPlayClip(AudioClips.Dash);
					//AudioManager.singleton.PlayClip(evadeSound);
					evadeDelayTime = evadeDelay;
					evadeTime = evadeDuration;
					evadeCooldownTime = evadeCooldown;
					arenaManager.evadeCooldown.fillAmount = 0;
					syncAnimator.SetTrigger("B");
				} else if (Input.GetButtonDown("X")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("X")) {
					//holdDuration = holdButton;
					//Debug.Log(holdDuration + " " + (holdDuration >= holdMinDuration));
					syncAnimator.SetTrigger("X");
				} else if (Input.GetButtonDown("Y")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("Y")) {
					//holdDuration = holdButton;
					//Debug.Log(holdDuration + " " + (holdDuration >= holdMinDuration));
					syncAnimator.SetTrigger("Y");
				}

				if (Input.GetButton("LB") && !playerMove.isAttacking) {
					SetShield(true);
				} else if (robotModel.shield.activeSelf) {
					SetShield(false);
				}

				if (upgradeWheel) {
					if (Input.GetAxis("Triggers") < -0.01f) {
						if (!upgradeWheelActive) {
							upgradeWheel.gameObject.SetActive(true);
							upgradeWheelActive = true;
						}
					} else {
						upgradeWheel.gameObject.SetActive(false);
						upgradeWheelActive = false;
					}
				}
				playerMove.canRotateCamera = Input.GetAxis("Triggers") >= -0.01f;

				if (Input.GetButtonDown("RS")) {
					if (lockCameraRobot) {
						DisableLockCamera();
					} else {
						if (Physics.BoxCast(transform.position, new Vector3(7, 7, 0.1f), transform.forward, out hit, Quaternion.identity, lockCameraMaxDistance, LayerMask.GetMask("Players"))) {
							lockCameraRobot = hit.transform.gameObject.GetComponent<Robot>();
						} else if (Physics.BoxCast(transform.position, new Vector3(0.1f, 7, 7), transform.forward, out hit, Quaternion.identity, lockCameraMaxDistance, LayerMask.GetMask("Players"))) {
							lockCameraRobot = hit.transform.gameObject.GetComponent<Robot>();
						}
						// in the boss round, you can lock only the boss
						if (lockCameraRobot && lockCameraRobot.health > 0 && (!MatchManager.singleton.bossRound || player.roundWinner >= 2 || lockCameraRobot.player.roundWinner >= 2)) {
							lockCameraRobot.marker.enabled = true;
						} else {
							lockCameraRobot = null;
						}
					}
				}

				if (lockCameraRobot) {
					if (Vector3.Distance(transform.position, lockCameraRobot.transform.position) > lockCameraMaxDistance || lockCameraRobot.health <= 0) {
						DisableLockCamera();
					} else {
						transform.LookAt(lockCameraRobot.transform);
						Vector2 screenPoint = Camera.main.WorldToScreenPoint(lockCameraRobot.transform.position + new Vector3(0, 1, 0));
						Vector2 canvasPos;
						RectTransformUtility.ScreenPointToLocalPointInRectangle(arenaManager.canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);
						lockCameraRobot.GetComponent<Robot>().marker.transform.localPosition = canvasPos;
					}
				}

				// Ion particles
				if (Input.GetButton("RB") && Input.GetAxis("Triggers") <= 0.01f) {
					SetIon(1);
				} else if (Input.GetAxis("Triggers") > 0.01f && !Input.GetButton("RB")) {
					SetIon(-1);
				} else {
					SetIon(0);
				}
			}
			animator.SetFloat("WalkH", playerMove.walkH);
			animator.SetFloat("WalkV", playerMove.walkV);
		}
	}
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward * lockCameraMaxDistance);
		Gizmos.DrawWireSphere(transform.position + transform.forward * lockCameraMaxDistance, 5);
	}

	short currentIon;
	public void SetIon(short ion) {
		if (currentIon != ion) {
			CmdSetIon(ion);
			currentIon = ion;
		}
	}
	[Command]
	void CmdSetIon(short ion) {
		RpcSetIon(ion);
	}
	[ClientRpc]
	void RpcSetIon(short ion) {
		switch (ion) {
			case 1:
				ionParticle.GetComponent<Renderer>().material = ionPlus;
				break;
			case -1:
				ionParticle.GetComponent<Renderer>().material = ionMinus;
				break;
			default:
				ionParticle.GetComponent<Renderer>().material = ionNull;
				break;
		}
	}

	public void DisableLockCamera() {
		if (lockCameraRobot) {
			lockCameraRobot.marker.enabled = false;
			lockCameraRobot = null;
			rigidbody.angularVelocity = Vector3.zero;
			transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
		}
	}

	public void SetShield(bool value) {
		if (robotModel.shield.activeSelf != value) {
			CmdSetShield(value);
		}
	}
	[Command]
	void CmdSetShield(bool value) {
		RpcSetShield(value);
	}
	[ClientRpc]
	void RpcSetShield(bool value) {
		//isGuardOn = value;
		robotModel.shield.SetActive(value);
	}

	[SyncVar]
	short comboScore;
	[SyncVar]
	float comboScoreDuration = 0;
	[Command]
	public void CmdIncreaseComboScore() {
		comboScore = (short)(comboScore * 1.5f);
		if (comboScore > 6) {
			comboScore = 6;
		}
		comboScoreDuration = comboDelay;
	}
	[Command]
	public void CmdResetComboScore() {
		comboScore = initialComboScore;
	}

	void UpdateHealthSlider(float value) {
		health = value;
		healthSlider.value = value / healthMax;
	}
	[Command]
	public void CmdUpdateHealthValue(float newHealth) {
		if (newHealth > healthMax) {
			newHealth = healthMax;
		}
		if (newHealth <= 0) {
			player.deathCount++;
			CmdPlayClip(AudioClips.Destroyed);
			//AudioManager.singleton.PlayClip(destroyedSound);
			//syncTransform.CmdSetValues(new Vector3(0, -100, 0), Quaternion.identity);
			RpcRespawn();
		}
		health = (short)newHealth;
	}
	public void UpdateHealth(float variation, bool isHeavy = false) {
		float newHealth = health + variation;
		if (newHealth <= 0) {
			DisableLockCamera();
		}
		CmdUpdateHealthValue(newHealth);
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
				if (leftHandCounter < 0) {
					leftHandCounter = 0;
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
				if (rightHandCounter < 0) {
					rightHandCounter = 0;
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
				if (leftFootCounter < 0) {
					leftFootCounter = 0;
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
				if (rightFootCounter < 0) {
					rightFootCounter = 0;
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
				if (headCounter < 0) {
					headCounter = 0;
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
	public void CmdEnableCollider(bool enableLeftHand, bool enableRightHand, bool enableLeftFoot, bool enableRightFoot, bool enableHead, bool breakGuard, bool pushBack, float hitDelay) {
		StartCoroutine(EnableCollider(enableLeftHand, enableRightHand, enableLeftFoot, enableRightFoot, enableHead, breakGuard, pushBack, hitDelay));
	}
	IEnumerator EnableCollider(bool enableLeftHand, bool enableRightHand, bool enableLeftFoot, bool enableRightFoot, bool enableHead, bool breakGuard, bool pushBack, float hitDelay) {
		if (hitDelay > 0) {
			yield return new WaitForSeconds(hitDelay);
		}
		Debug.Log(player.name + " has enabled a collider.");
		this.breakGuard = breakGuard;
		this.pushBack = pushBack;
		if (enableLeftHand) {
			ActivateBodyPart(BodyPartCollider.leftHand, true);
			leftHand.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableRightHand) {
			ActivateBodyPart(BodyPartCollider.rightHand, true);
			rightHand.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableLeftFoot) {
			ActivateBodyPart(BodyPartCollider.leftFoot, true);
			leftFoot.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableRightFoot) {
			ActivateBodyPart(BodyPartCollider.rightFoot, true);
			rightFoot.GetComponent<BodyPartHitter>().hitters.Clear();
		}
		if (enableHead) {
			ActivateBodyPart(BodyPartCollider.head, true);
			head.GetComponent<BodyPartHitter>().hitters.Clear();
		}
	}

	[Command]
	public void CmdDisableCollider(bool enableLeftHand, bool enableRightHand, bool enableLeftFoot, bool enableRightFoot, bool enableHead) {
		Debug.Log(player.name + " has disabled a collider.");
		if (enableLeftHand) {
			ActivateBodyPart(BodyPartCollider.leftHand, false);
		}
		if (enableRightHand) {
			ActivateBodyPart(BodyPartCollider.rightHand, false);
		}
		if (enableLeftFoot) {
			ActivateBodyPart(BodyPartCollider.leftFoot, false);
		}
		if (enableRightFoot) {
			ActivateBodyPart(BodyPartCollider.rightFoot, false);
		}
		if (enableHead) {
			ActivateBodyPart(BodyPartCollider.head, false);
		}
	}

	[Command]
	public void CmdGetHitted(GameObject hitterGO, Vector3 position, GameObject particle) {
		Robot hitter = hitterGO.GetComponent<Robot>();
		Debug.Log(hitter.name + " hits " + name + " " + hitter.pushBack);
		// avoid "friendly fire" in boss round
		if (MatchManager.singleton.bossRound && player.roundWinner < 2 && hitter.player.roundWinner < 2) {
			return;
		}
		if (!robotModel.shield.activeSelf || hitter.breakGuard || hitter.player.roundWinner >= 2 ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) && !hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name)) {
			GameObject effect = Instantiate(hitter.hitEffect);
			effect.transform.position = position;
			effect.transform.localScale = Vector3.one;
			NetworkServer.Spawn(effect);
			syncAnimator.SetTrigger("Reaction");
			if (!particle) {
				CmdPlayClip(AudioClips.Hit);
			} else if (particle.name == fireParticle.name) {
				CmdPlayClip(AudioClips.Fire);
			} else if (particle.name == iceParticle.name) {
				CmdPlayClip(AudioClips.Ice);
			} else if (particle.name == lightningParticle.name) {
				CmdPlayClip(AudioClips.Lightning);
			} else if (particle.name == sonicParticle.name) {
				CmdPlayClip(AudioClips.Sonic);
			} else {
				Debug.LogError("Sound not found for " + particle.name);
			}
			float damage = hitter.attack * 2 * (1 - (defense * 5) / 100);
			if (health - damage <= 0) {
				if (!MatchManager.singleton.bossRound) {
					hitter.UpdateHealth(hitter.healthMax / 3);
				}
				if (hitter.lockCameraRobot) {
					hitter.DisableLockCamera();
				}
			}
			if (hitter.pushBack) {
				Debug.Log("Push");
				RpcAddForce(hitter.transform.forward * 8.5f * 50, ForceMode.Impulse);
			}
			UpdateHealth(-damage);
			if (!MatchManager.singleton.bossRound) {
				hitter.player.scraps += 3;
			}
			Debug.Log(hitter.player.name + " current combo score: " + hitter.comboScore);
			//hitter.player.score += (short)hitter.comboScore;
			hitter.roundScore += hitter.comboScore;
			SetShield(false);
			CmdDisableCollider(true, true, true, true, true);
		}
	}

	[Command]
	public void CmdAddForce(Vector3 force, ForceMode mode) {
		RpcAddForce(force, mode);
	}
	[ClientRpc]
	void RpcAddForce(Vector3 force, ForceMode mode) {
		if (isLocalPlayer) {
			rigidbody.AddForce(force, mode);
		}
	}

	[ClientRpc]
	public void RpcRespawn() {
		if (player) {
			Debug.Log(player.name + " death position is " + transform.position);
			StartCoroutine(RespawnCoroutine());
		}
	}
	float respawnWaiting;
	IEnumerator RespawnCoroutine() {
		DisableLockCamera();
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).gameObject != keepOnRespawn) {
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		CmdSetPaused(gameObject, true);
		if (MatchManager.singleton.bossRound) {
			if (isLocalPlayer && !player.isAgent) {
				arenaManager.title.text = arenaManager.youDestroyed;
			}
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
			arenaManager.title.text = "";
		}
		CmdUpdateHealthValue(healthMax);
		CmdSetPaused(gameObject, false);

		//CmdSpawn();
		Transform spawn = FindObjectsOfType<SpawnPoint>()[player.playerID - 1].transform; //NetworkManager.singleton.GetStartPosition();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;
		rigidbody.velocity = Vector3.zero;
		//syncTransform.CmdSetPosition(spawn.position);
		//syncTransform.CmdSetRotation(spawn.rotation);
		Debug.Log(player.name + " spawn at " + transform.position);
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(true);
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
	[Command]
	public void CmdAddTemporaryUpgrade(int ID) {
		Debug.Log("CmdAddTemporaryUpgrade");
		Upgrade u = Upgrades.temporary[ID];
		if (u.price <= player.scraps) {
			player.scraps -= (short)u.price;
			RpcAddTemporaryUpgrade(ID);
			Debug.Log("Upgrade " + u.name + " mounted!");
		} else {
			Debug.Log("Not enough scraps!");
		}
	}
	[ClientRpc]
	public void RpcAddTemporaryUpgrade(int ID) {
		Debug.Log("RpcAddTemporaryUpgrade");
		Upgrade u = Upgrades.temporary[ID];
		int index = (int)u.type;
		if (index < upgrades.Length) {
			if (upgrades[index] != 0) {
				Upgrades.temporary[upgrades[index]].OnRemove(this);
			}
			upgrades[index] = ID;
		}
		u.OnAdd(this);
		Debug.Log("Upgrade " + u.name + " mounted!");
	}
	List<GameObject> handsParticles = new List<GameObject>();
	List<GameObject> feetParticles = new List<GameObject>();
	GameObject bodyParticles;
	public void RefreshUpgradeParticle(UpgradeElements element, BodyPart bodyPart) {
		GameObject particle = null;
		switch (element) {
			case UpgradeElements.Fire:
				particle = fireParticle;
				break;
			case UpgradeElements.Ice:
				particle = iceParticle;
				break;
			case UpgradeElements.Lightning:
				particle = lightningParticle;
				break;
			case UpgradeElements.Sonic:
				particle = sonicParticle;
				break;
			default:
				Debug.LogError("Unrecognized element: " + element);
				break;
		}
		Debug.Log(player.name + " mounts " + particle.name + " on " + bodyPart);
		Vector3 scale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
		switch (bodyPart) {
			case BodyPart.Hands:
				if (particle != handsParticle) {
					handsParticle = particle;
				}
				// Remove existing particles.
				for (int i = 0; i < handsParticles.Count; i++) {
					Destroy(handsParticles[i]);
				}
				handsParticles.Clear();

				GameObject leftParticle = Instantiate(handsParticle);
				GameObject rightParticle = Instantiate(handsParticle);
				handsParticles.Add(leftParticle);
				handsParticles.Add(rightParticle);

				leftParticle.transform.SetParent(leftHand.transform);
				rightParticle.transform.SetParent(rightHand.transform);

				leftParticle.transform.localPosition = Vector3.zero;
				leftParticle.transform.localScale = scale;
				rightParticle.transform.localPosition = Vector3.zero;
				rightParticle.transform.localScale = scale;

				leftHand.GetComponent<BodyPartHitter>().particle = handsParticle;
				rightHand.GetComponent<BodyPartHitter>().particle = handsParticle;
				break;
			case BodyPart.Feet:
				if (particle != feetParticle) {
					feetParticle = particle;
				}
				// Remove existing particles.
				for (int i = 0; i < feetParticles.Count; i++) {
					Destroy(feetParticles[i]);
				}
				feetParticles.Clear();

				leftParticle = Instantiate(feetParticle);
				rightParticle = Instantiate(feetParticle);
				feetParticles.Add(leftParticle);
				feetParticles.Add(rightParticle);

				leftParticle.transform.SetParent(leftFoot.transform);
				rightParticle.transform.SetParent(rightFoot.transform);

				leftParticle.transform.localPosition = Vector3.zero;
				leftParticle.transform.localScale = scale;
				rightParticle.transform.localPosition = Vector3.zero;
				rightParticle.transform.localScale = scale;

				leftFoot.GetComponent<BodyPartHitter>().particle = feetParticle;
				rightFoot.GetComponent<BodyPartHitter>().particle = feetParticle;
				break;
			default:
				if (particle != bodyParticle) {
					bodyParticle = particle;
				}
				// Remove existing particles.
				Destroy(bodyParticles);

				bodyParticles = Instantiate(bodyParticle);

				bodyParticles.transform.SetParent(body.transform.GetChild(0));

				bodyParticles.transform.localPosition = Vector3.zero;
				bodyParticles.transform.localScale = scale * 1.5f;

				head.GetComponent<BodyPartHitter>().particle = bodyParticle;
				break;
		}
		if (particle.name == fireParticle.name) {
			CmdPlayClip(AudioClips.Fire);
		} else if (particle.name == iceParticle.name) {
			CmdPlayClip(AudioClips.Ice);
		} else if (particle.name == lightningParticle.name) {
			CmdPlayClip(AudioClips.Lightning);
		} else if (particle.name == sonicParticle.name) {
			CmdPlayClip(AudioClips.Sonic);
		}
	}

	[Command]
	public void CmdPlayClip(AudioClips clipIndex) {
		RpcPlayClip(clipIndex);
	}
	[ClientRpc]
	public void RpcPlayClip(AudioClips clipIndex) {
		if (clips.Length > (int)clipIndex) {
			GetComponent<AudioSource>().clip = clips[(int)clipIndex];
			GetComponent<AudioSource>().Play();
		}
	}
}
