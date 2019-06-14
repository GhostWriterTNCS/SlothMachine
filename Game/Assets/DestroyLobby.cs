using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLobby : MonoBehaviour {
	void Start() {
		if (FindObjectOfType<Prototype.NetworkLobby.LobbyManager>()) {
			DestroyImmediate(FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().gameObject);
		}
		while (FindObjectsOfType<Player>().Length > 0) {
			DestroyImmediate(FindObjectsOfType<Player>()[0].gameObject);
		}
	}
}
