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
	[Space]
	public NetworkArenaManager networkArenaManager;
	public bool arenaReady;

	public void Start() {
		arenaReady = false;
		StartCoroutine(SetupCoroutine());
	}

	IEnumerator SetupCoroutine() {
		/*while (!FindObjectOfType<Robot>() || !FindObjectOfType<Robot>().player) {
			yield return new WaitForSeconds(0.05f);
		}*/
		title.text = roundX.Replace("#", MatchManager.singleton.roundCounter.ToString());
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
		FindObjectOfType<NetworkArenaManager>().CmdRoundOver();
	}

	IEnumerator LoadScene(string scene) {
		yield return new WaitForSeconds(5);
		NetworkManager.singleton.ServerChangeScene(scene);
	}
}
