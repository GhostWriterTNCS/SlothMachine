using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultManager : MonoBehaviour {
	public Transform container;
	public Button backToStart;
	public GameObject matchResultPrefab;

	void Start() {
		backToStart.gameObject.SetActive(false);
		foreach (Player p in FindObjectsOfType<Player>()) {
			GameObject newPlayer = Instantiate(matchResultPrefab);
			PlayerResult res = newPlayer.GetComponent<PlayerResult>();
			res.playerGO = gameObject;
		}
	}
}
