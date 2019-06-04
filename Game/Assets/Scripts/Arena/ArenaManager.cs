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
	public Transform minimap;
	[Space]
	public Text title;
	public string roundX;
	public string finalRound;
	public string roundWinnerIs;
	public string matchOver;
	public string respawnIn;
	public string youDefeated;
	[Space]
	public GameObject[] arenaPrefabs;
	public GameObject bossArena;
	[Space]
	public NetworkArenaManager networkArenaManager;
	public bool arenaReady;

	public void Start() {
		arenaReady = false;
		countdown.text = "";
		scrapsCounter.text = "";
	}
}
