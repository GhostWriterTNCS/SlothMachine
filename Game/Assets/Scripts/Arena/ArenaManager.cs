﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour {
	public GameObject countdown;
	public GameObject leaderboard;
	public UpgradeWheel upgradeWheel;
	public GameObject arenaBoxPrefab;
	public Canvas canvas;
	public Text roundWinnerText;

	public void RoundOver() {
		int scoreMax = 0;
		Player roundWinner = null;
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (p.score > scoreMax) {
				scoreMax = p.score;
				roundWinner = p;
			}
		}
		roundWinner.roundWinner++;
		countdown.SetActive(false);
		roundWinnerText.text = roundWinnerText.text.Replace("#", roundWinner.name);
		roundWinnerText.gameObject.SetActive(true);
		if (roundWinner.roundWinner < 2) {
			StartCoroutine(LoadAuction(GameScenes.Auction));
		} else {
			StartCoroutine(LoadAuction(GameScenes.Arena));
		}
	}

	IEnumerator LoadAuction(string scene) {
		yield return new WaitForSeconds(5);
		NetworkManager.singleton.ServerChangeScene(scene);
	}
}
