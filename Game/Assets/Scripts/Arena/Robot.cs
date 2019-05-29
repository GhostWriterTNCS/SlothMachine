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
	public GameObject fireParticle;
	public GameObject lightningParticle;
	public GameObject iceParticle;
	[Space]
	public Material ionPlus;
	public Material ionMinus;
	public Material ionNull;
	public ParticleSystem ionParticle;
	[Space]
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
	[Space]
	public Collider leftHand;
	public Collider rightHand;
	public Collider leftFoot;
	public Collider rightFoot;
	public Collider head;
	public Collider body;
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
		while (!playerGO) {
			yield return new WaitForSeconds(0.05f);
		}

		player = playerGO.GetComponent<Player>();
		transform.SetParent(player.transform);
		arenaManager = FindObjectOfType<ArenaManager>();
		if (isLocalPlayer) {
			nameText.text = "";
			scrapsCounter = arenaManager.scrapsCounter;
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
		upgradeWheel = FindObjectOfType<UpgradeWheel>();
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
	Robot lockCameraRobot;
	void Update() {
		if (paused) {
			return;
		}
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
				scrapsCounter.text = player.scraps + " scraps";
				holdButton += Time.deltaTime;
				if (Input.GetButtonDown("A")) {
					holdButton = 0;
				} else if (Input.GetButtonUp("A") && Input.GetAxis("Triggers") >= -0.01f) {
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

				upgradeWheel.gameObject.SetActive(Input.GetAxis("Triggers") < -0.01f);
				playerMove.canRotateCamera = Input.GetAxis("Triggers") >= -0.01f;

				if (Input.GetButtonDown("RS")) {
					if (lockCameraRobot) {
						lockCameraRobot.marker.enabled = false;
						lockCameraRobot = null;
					} else {
						RaycastHit hit;
						if (Physics.BoxCast(transform.position, new Vector3(3, 3, 3), transform.TransformDirection(Vector3.forward), out hit, Quaternion.identity, 30, 9)) {
							lockCameraRobot = hit.transform.gameObject.GetComponent<Robot>();
							if (lockCameraRobot) {
								lockCameraRobot.marker.enabled = true;
							}
						}
					}
				}
				if (lockCameraRobot) {
					if (Vector3.Distance(transform.position, lockCameraRobot.transform.position) > 10) {
						lockCameraRobot.marker.enabled = false;
						lockCameraRobot = null;
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
		Debug.Log(hitter.name + " hits " + name);
		if (!isGuardOn ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) && !hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionNull.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) ||
			ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionMinus.name) && hitter.ionParticle.GetComponent<Renderer>().material.name.StartsWith(ionPlus.name)) {
			GameObject effect = Instantiate(hitter.hitEffect);
			effect.transform.position = position;
			effect.transform.localScale = Vector3.one;
			NetworkServer.Spawn(effect);
			animator.SetTrigger("Reaction");
			float damage = 5 * hitter.attack / defense;
			if (health - damage <= 0) {
				hitter.UpdateHealth(hitter.healthMax / 3);
				if (hitter.lockCameraRobot) {
					hitter.lockCameraRobot.marker.enabled = false;
					hitter.lockCameraRobot = null;
				}
			}
			UpdateHealth(-damage);
			hitter.player.scraps += 3;
			Debug.Log(hitter.player.name + " Current combo score: " + hitter.comboScore);
			hitter.player.score += (int)hitter.comboScore;
			hitter.roundScore += (int)hitter.comboScore;
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

					leftParticle.transform.localPosition = leftHand.transform.localPosition;
					leftParticle.transform.localScale = scale;
					rightParticle.transform.localPosition = rightFoot.transform.localPosition;
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

					leftParticle.transform.localPosition = leftFoot.transform.localPosition;
					leftParticle.transform.localScale = scale;
					rightParticle.transform.localPosition = rightFoot.transform.localPosition;
					rightParticle.transform.localScale = scale;
				}
				break;
			default:
				if (particle != bodyParticle) {
					bodyParticle = particle;
					// Remove existing particles.
					Destroy(bodyParticles);

					GameObject bodyParticleLocal = Instantiate(particle);
					feetParticles.Add(bodyParticleLocal);

					bodyParticleLocal.transform.SetParent(body.transform);

					bodyParticleLocal.transform.localPosition = body.transform.localPosition;
					bodyParticleLocal.transform.localScale = scale * 1.5f;
				}
				break;
		}

	}
}
