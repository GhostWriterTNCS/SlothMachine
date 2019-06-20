using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour {
	public Text countdown;
	public int roundDuration;
	public GameObject leaderboard;
	public UpgradeWheel upgradeWheel;
	public GameObject arenaBoxPrefab;
	public Canvas canvas;
	public GameObject bottomBar;
	public Text scrapsCounter;
	public PauseMenu pauseMenu;
	public Transform minimap;
	public Image evadeCooldown;
	[Space]
	public Text title;
	public string roundX;
	public string finalRound;
	public string finalRoundBoss;
	public string finalRoundOthers;
	public string roundWinnerIs;
	public string matchOver;
	public string respawnIn;
	public string youDestroyed;
	public string youWin;
	public string bossDefeated;
	public string youLost;
	[Space]
	public GameObject[] arenaPrefabs;
	public GameObject bossArena;
	[Space]
	public bool arenaReady;

	NetworkArenaManager networkArenaManager;

	void Start() {
		arenaReady = false;
		countdown.text = "";
		bottomBar.SetActive(false);
		title.gameObject.SetActive(true);
		title.text = "";
	}

	void Update() {
		if (!networkArenaManager) {
			networkArenaManager = FindObjectOfType<NetworkArenaManager>();
		} else {
			countdown.text = networkArenaManager.roundTime;
		}
	}
}
