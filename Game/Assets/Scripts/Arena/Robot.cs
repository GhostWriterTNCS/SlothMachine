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
	[SyncVar(hook = "UpdateHealthSlider")]
	public float health;

	RobotModel robotModel;
	Animator animator;
	NetworkAnimator networkAnimator;
	PlayerCamera playerCamera;
	PlayerMove playerMove;
	Rigidbody rigidbody;

	void Start() {
		player = GetComponentInParent<Player>();
		GameObject model = Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName), transform);
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
		playerCamera = GetComponent<PlayerCamera>();
		playerMove = GetComponent<PlayerMove>();
		rigidbody = GetComponent<Rigidbody>();

		maxHealth = (int)(100 * (1 + (robotModel.health - 3) / 10f));
		CmdUpdateHealthValue(maxHealth);
		leftHand.enabled = false;
		rightHand.enabled = false;
	}

	float holdButton = 0;
	//[SyncVar]
	float holdDuration = 0;

	Vector3 evadeDirection;
	float evadeTime = 0;
	Transform lockCamera;
	void Update() {
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
					playerMove.moveSpeedMultiplier = 0.5f;
				} else if (Input.GetButtonUp("LB")) {
					animator.SetBool("LB", false);
					playerMove.moveSpeedMultiplier = 1;
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

	public void GetHitted(Robot hitter) {
		Debug.Log(name + " hitted by " + hitter.name + " | " + hitter.holdDuration);
		animator.SetTrigger("Reaction");
		UpdateHealth(-5 * hitter.robotModel.attack / (float)robotModel.defense);
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

			leftParticle.transform.localPosition = leftHand.center;
			leftParticle.transform.localScale = Vector3.one;
			rightParticle.transform.localPosition = rightFoot.center;
			rightParticle.transform.localScale = Vector3.one;
		}
	}
}
