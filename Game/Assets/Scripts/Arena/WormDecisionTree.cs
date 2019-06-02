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
	public ParticleSystem particle;
	public GameObject wormPrefab;
	private DecisionTree dt;
	private Robot playerTarget;
	private Rigidbody myRigidbody;
	private float timer;
	private float timerArena;
	private int spawnTime;

	// Start is called before the first frame update
	void Start() {
		// Define actions
		DTAction a1 = new DTAction(moveToBoundaries);
		DTAction a2 = new DTAction(attackRandomly);

		// Define decisions
		DTDecision d1 = new DTDecision(checkLastAttack);
		DTDecision d2 = new DTDecision(checkRobotAround);


		// Link action with decisions
		d1.AddLink(false, d2);
		d1.AddLink(true, a1);

		d2.AddLink(true, a2);

		// Setup my DecisionTree at the root node
		dt = new DecisionTree(d1);
		myRigidbody = GetComponent<Rigidbody>();
		particle = GetComponent<ParticleSystem>();
		StartCoroutine(Hunt());

		spawnTime = FindObjectOfType<ArenaManager>().roundDuration / 2;
	}

	Transform destination;
	private void Update() {
		if (destination) {

			Vector3 verticalAdj = new Vector3(destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			// we keep only option a
			transform.LookAt(verticalAdj);
			Rigidbody rb = GetComponent<Rigidbody>();
			myRigidbody.MovePosition(myRigidbody.position + transform.forward * moveSpeed * Time.deltaTime);
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
		if (playerTarget) {
			return true;
		}
		//Debug.Log("checkRobotAround");
		GetComponent<Collider>().enabled = true;
		particle.Play();
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


	//move randomly for an amount of time near the boundaries of the map
	public object moveToBoundaries(object o) {
		GetComponent<Collider>().enabled = false;
		particle.Stop();
		return null;
	}


	private void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Robot>()) {
			timerArena = FindObjectOfType<ArenaManager>().countdown.GetComponentInChildren<Countdown>().seconds;
			//Debug.Log(timerArena);
			if (timerArena <= spawnTime) {
				Debug.Log("timerArena < " + spawnTime);
				FindObjectOfType<NetworkArenaManager>().CmdSpawnWorm(wormPrefab, new Vector3(destination.position.x, destination.position.y - 10, destination.position.z));
			}
			Debug.Log("colpito");
			playerTarget = null;
			destination = null;
			timer = waitDuration;
		}
	}
}
