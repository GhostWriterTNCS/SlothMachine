using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkipToBossArena : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.F6)) {
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
