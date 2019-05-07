using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	[SyncVar]
	public string robotModel = "Dozzer";
	public GameObject robot;

	void Awake() {
		name = "Player " + playerControllerId;
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (p != this && p.netId == netId) {
				Debug.Log("Player " + netId + " already exists.");
				Destroy(gameObject);
				return;
			}
		}
		DontDestroyOnLoad(gameObject);
	}
}
