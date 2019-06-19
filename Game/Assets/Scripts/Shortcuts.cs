using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Shortcuts : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.F5)) {
			NetworkManager.singleton.ServerChangeScene(GameScenes.Auction);
		} else if (Input.GetKeyDown(KeyCode.F6)) {
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		} else if (Input.GetKeyDown(KeyCode.F7)) {
			int scoreMax = -1;
			Player roundWinner = null;
			foreach (Player p in FindObjectsOfType<Player>()) {
				p.robot.paused = true;
				if (p.robot.roundScore > scoreMax) {
					scoreMax = p.robot.roundScore;
					roundWinner = p;
				}
			}
			roundWinner.roundWinner = 2;
			MatchManager.singleton.bossRound = true;
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		}
	}
}
