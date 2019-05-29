using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour {
	public GameObject countdown;
	public GameObject leaderboard;
	public UpgradeWheel upgradeWheel;
	public GameObject arenaBoxPrefab;
	public Canvas canvas;
	public Text scrapsCounter;
	[Space]
	public Text title;
	public string roundX;
	public string finalRound;
	public string roundWinnerIs;

	public void Start() {
		StartCoroutine(SetupCoroutine());
	}

	IEnumerator SetupCoroutine() {
		title.text = roundX.Replace("#", 1.ToString());
		yield return new WaitForSeconds(2);
		title.gameObject.SetActive(false);
		CmdPauseAll(false);
	}

	//[Command]
	public void CmdPauseAll(bool value) {
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			r.paused = value;
		}
	}

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
		title.text = roundWinnerIs.Replace("#", roundWinner.name);
		title.gameObject.SetActive(true);
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
