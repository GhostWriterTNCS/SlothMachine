using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour {
	public Text countdown;
	public int roundDuration;
	public GameObject leaderboard;
	public UpgradeWheel upgradeWheel;
	public GameObject arenaBoxPrefab;
	public Canvas canvas;
	public Text scrapsCounter;
	public PauseMenu pauseMenu;
	[Space]
	public Text title;
	public string roundX;
	public string finalRound;
	public string roundWinnerIs;
	public string respawnIn;
	[Space]
	public GameObject[] arenaPrefabs;
	[Space]
	public NetworkArenaManager networkArenaManager;
	public bool arenaReady;

	public void Start() {
		arenaReady = false;
		countdown.text = "";
		scrapsCounter.text = "";
		//StartCoroutine(SetupCoroutine());
	}

	/*IEnumerator SetupCoroutine() {
		/*while (!FindObjectOfType<Robot>() || !FindObjectOfType<Robot>().player) {
			yield return new WaitForSeconds(0.05f);
		}*/
	/*if (MatchManager.singleton.roundCounter > 0) {
		title.text = roundX.Replace("#", MatchManager.singleton.roundCounter.ToString());
	} else {
		title.text = finalRound;
	}
	yield return new WaitForSeconds(2);
	title.gameObject.SetActive(false);
	CmdPauseAll(false);
}*/

	//[Command]
	/*public void CmdPauseAll(bool value) {
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			r.paused = value;
		}
	}*/

	/*public void RoundOver() {
		FindObjectOfType<NetworkArenaManager>().CmdRoundOver();
	}

	IEnumerator LoadScene(string scene) {
		yield return new WaitForSeconds(5);
		NetworkManager.singleton.ServerChangeScene(scene);
	}*/
}
