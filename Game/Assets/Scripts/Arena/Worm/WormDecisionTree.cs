using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WormDecisionTree : NetworkBehaviour {

	[Range(0f, 20f)] public float range = 5f;
	[Range(0f, 10f)] public float moveSpeed = 3f;
	public float reactionTime = 2f;
	public float waitDuration = 10;
	public ParticleSystem standardParticle;
	public ParticleSystem aggressiveParticle;
	public BoxCollider triggerCollider;
	public GameObject wormPrefab;
	private DecisionTree dt;
	private Robot playerTarget;
	private Rigidbody myRigidbody;
	private float timer;
	private float timerArena;
	private int spawnTime;
	public float radius = 5.0F;
	public float power = 10.0F;

	// Start is called before the first frame update
	void Start() {
		// Define actions
		DTAction a1 = new DTAction(moveToBoundaries);
		DTAction a2 = new DTAction(attackRandomly);
		DTAction a3 = new DTAction(waitFreezen);

		// Define decisions
		DTDecision d1 = new DTDecision(checkLastAttack);
		DTDecision d2 = new DTDecision(checkRobotAround);


		// Link action with decisions
		d1.AddLink(false, d2);
		d1.AddLink(true, a3);

		d2.AddLink(true, a2);
		d2.AddLink(false, a1);

		// Setup my DecisionTree at the root node
		dt = new DecisionTree(d1);
		myRigidbody = GetComponent<Rigidbody>();
		aggressiveParticle.Stop();
		StartCoroutine(Hunt());

		spawnTime = FindObjectOfType<ArenaManager>().roundDuration / 2;
	}

	public IEnumerator RespawnCoroutine() {
		yield return new WaitForSeconds(1);
		transform.position = new Vector3(0, -100, 0);
		yield return new WaitForSeconds(5);
		float x = Random.Range(225f, 310f);
		float z = Random.Range(190f, 275f);
		float y = FindObjectOfType<Terrain>().terrainData.GetHeight((int)x, (int)z) + 3;
		transform.position = new Vector3(x, y, z);
	}

	Transform destination;
	Vector3 randomPosition;
	private void Update() {
		if (destination) {
			Vector3 verticalAdj = new Vector3(destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			// we keep only option a
			transform.LookAt(verticalAdj);
			Rigidbody rb = GetComponent<Rigidbody>();
			myRigidbody.MovePosition(myRigidbody.position + transform.forward * moveSpeed * Time.deltaTime);
		} else if (randomPosition != Vector3.zero) {
			if ((randomPosition - transform.position).magnitude >= 2f) {
				Vector3 toDestination = (randomPosition - transform.position);
				transform.LookAt(randomPosition);
				Rigidbody rb = GetComponent<Rigidbody>();
				myRigidbody.MovePosition(myRigidbody.position + transform.forward * moveSpeed * Time.deltaTime);
			} else {
				//Debug.Log("Verme vicino alla posizione");
				randomPosition = Vector3.zero;
				//Debug.Log("nuova posizione settata a zero"+ randomPosition);
				timer = waitDuration;
				StartCoroutine(RespawnCoroutine());
			}

		}
	}

	// Take decision every interval, run forever
	public IEnumerator Hunt() {
		while (true) {
			// Debug.Log("Worm");
			dt.walk();
			yield return new WaitForSeconds(reactionTime);
		}
	}

	//DECISIONS
	//check how many second he attacks a player
	public object checkLastAttack(object o) {
		timer -= reactionTime;
		if (timer <= 0) {
			return false;
		}
		return true;
	}

	//check the nearest player in the range
	public object checkRobotAround(object o) {
		timerArena = FindObjectOfType<NetworkArenaManager>().roundDuration;

		if (timerArena <= spawnTime) {
			if (standardParticle.isPlaying) {
				standardParticle.Stop();
				aggressiveParticle.Play();
				triggerCollider.size = new Vector3(2, 2, 2);
			}
			if (playerTarget) {
				return true;
			}
			//Debug.Log("checkRobotAround");
			GetComponent<Collider>().enabled = true;
			System.Random rnd = new System.Random();
			Robot[] arr = FindObjectsOfType<Robot>().OrderBy(x => rnd.Next()).ToArray();
			foreach (Robot robot in arr) {
				if ((robot.transform.position - transform.position).magnitude <= range) {
					playerTarget = robot;
					return true;
				}
			}

			//attack the player who has the highest score
			int scoreMax = -1;
			foreach (Robot robot in arr) {
				if (robot.player && robot.player.score > scoreMax) {
					scoreMax = robot.player.score;
					playerTarget = robot;
				}
			}
			return true;
		} else {
			//Debug.Log("Sono in checkRobotAround e ritorno false perche non ancora meta tempo");
			return false;
		}

	}


	//ACTIONS
	//attack a enemy random in the range
	public object attackRandomly(object o) {
		if (playerTarget) {
			//Debug.Log("attackRandomly");
			//myRigidbody.MovePosition(playerTarget.transform.position);
			destination = playerTarget.transform;
		}
		return null;
	}

	public object waitFreezen(object o) {

		return null;
	}


	//move randomly for an amount of time near the boundaries of the map
	public object moveToBoundaries(object o) {
		//GetComponent<Collider>().enabled = false;
		//particle.Stop();
		if (randomPosition != Vector3.zero) {
			//Debug.Log("MoveToBoundaries la condizione randomPosition != Vector3.zero "+ (randomPosition != Vector3.zero));
			return true;
		}
		GetComponent<Collider>().enabled = true;
		float x = Random.Range(225f, 310f);
		float z = Random.Range(190f, 275f);
		float y = FindObjectOfType<Terrain>().terrainData.GetHeight((int)x, (int)z);

		randomPosition = new Vector3(x, y, z);
		//Debug.Log("Posizione del verme " + randomPosition);
		return null;
	}


	private void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Robot>()) {
			timerArena = FindObjectOfType<NetworkArenaManager>().roundDuration;
			//Debug.Log(timerArena);
			if (timerArena <= spawnTime) {
				Debug.Log("timerArena < " + spawnTime);
				FindObjectOfType<NetworkArenaManager>().CmdSpawnWorm(new Vector3(transform.position.x, transform.position.y - 10, transform.position.z));
			} else {
				other.GetComponent<Robot>().CmdAddForce(transform.forward * 9.5f * 50, ForceMode.Impulse); //forza era a 12.5
			}
			Debug.Log("colpito");

			playerTarget = null;
			destination = null;
			timer = waitDuration;
			StartCoroutine(RespawnCoroutine());
		}
	}
}
