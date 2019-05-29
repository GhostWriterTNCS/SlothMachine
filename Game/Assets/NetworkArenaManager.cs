using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkArenaManager : NetworkBehaviour {
	ArenaManager arenaManager;

	void Start() {
		arenaManager = FindObjectOfType<ArenaManager>();
		/*if (isServer)
			FixPlayersPositions();*/
	}

	[Command]
	public void CmdLoad() {

	}

	public void FixPlayersPositions() {
		StartCoroutine(FixPlayersPositionsCoroutine());
	}
	IEnumerator FixPlayersPositionsCoroutine() {
		while (!arenaManager.arenaReady) {
			yield return new WaitForSeconds(0.05f);
		}
		Debug.Log("FixPlayersPositionsCoroutine");
		foreach (Player player in FindObjectsOfType<Player>()) {
			while (!player.GetComponentInChildren<Robot>()) {
				yield return new WaitForSeconds(0.05f);
			}
			Debug.Log("Repos " + player.name);
			player.GetComponentInChildren<Robot>().CmdRespawn();
		}
	}

	[Command]
	public void CmdRoundOver() {
		int scoreMax = -1;
		Player roundWinner = null;
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (p.score > scoreMax) {
				scoreMax = p.score;
				roundWinner = p;
			}
		}
		roundWinner.roundWinner++;
		arenaManager.countdown.SetActive(false);
		arenaManager.title.text = arenaManager.roundWinnerIs.Replace("\\n", "\n").Replace("#", roundWinner.name);
		arenaManager.title.gameObject.SetActive(true);
		if (roundWinner.roundWinner < 2) {
			StartCoroutine(LoadScene(GameScenes.Arena));
		} else {
			StartCoroutine(LoadScene(GameScenes.Arena));
		}
	}

	IEnumerator LoadScene(string scene) {
		yield return new WaitForSeconds(5);
		NetworkManager.singleton.ServerChangeScene(scene);
	}
}
