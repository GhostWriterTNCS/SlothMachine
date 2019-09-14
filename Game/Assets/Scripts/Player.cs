using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {
	[SyncVar]
	public byte playerID;
	[SyncVar]
	public string robotName;
	[SyncVar]
	public short score;
	[SyncVar]
	public short scraps;
	[SyncVar]
	public byte roundWinner;
	[SyncVar]
	public byte deathCount;
	[SyncVar]
	public bool isAgent;
	[SyncVar]
	public Color color;

	public UpgradesBalance upgradesBalance;
	public UpgradesPrefer upgradesPrefer;
	public float[] favorites;

	public GameObject auctionPrefab;
	public GameObject auctionPlayerScraps;
	public GameObject networkAuctionManager;
	public GameObject arenaPrefab;
	public GameObject networkArenaManager;
	public GameObject matchResultPrefab;
	public Robot robot;

	public Pair[] upgrades = new Pair[] { null, null, null, null };
	[SyncVar]
	public bool upgradeAssigned;

	public Dictionary<Player, int> expectedMoney = new Dictionary<Player, int>();
	public Dictionary<Player, int> temporaryUpgradesCount = new Dictionary<Player, int>();
	public Dictionary<Player, float[]> expectedFavorites = new Dictionary<Player, float[]>();
	//public Dictionary<UpgradeBox, float> previousInterests = new Dictionary<UpgradeBox, float>();

	public readonly static short defaultScraps = 100;

	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		name = "Player " + playerID;// + (isAgent ? " (bot)" : "");
		if (isLocalPlayer && !isAgent) {
			name = "<b>" + name + "</b>";
		}
		score = 0;
		scraps = defaultScraps;
		roundWinner = 0;
		deathCount = 0;
		favorites = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
		CmdRespawn(gameObject);
	}

	[Command]
	public void CmdRespawn(GameObject go) {
		NetworkConnection conn = go.GetComponent<NetworkIdentity>().connectionToClient;
		if (!isAgent)
			NetworkServer.ReplacePlayerForConnection(conn, gameObject, 0);
		if (SceneManager.GetActiveScene().name == GameScenes.Arena) {
			Debug.Log("Spawn in arena.");
			//roundCounter++;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			GameObject newPlayer = Instantiate(arenaPrefab);
			Robot robot = newPlayer.GetComponent<Robot>();
			robot.playerGO = gameObject;
			robot.roundScore = 0;
			NetworkServer.Spawn(newPlayer);
			if (!isAgent) {
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
			}

			if (!FindObjectOfType<NetworkArenaManager>()) {
				GameObject NAM = Instantiate(networkArenaManager);
				NetworkServer.Spawn(NAM);
			}
		} else if (SceneManager.GetActiveScene().name == GameScenes.Auction) {
			//Debug.Log("Spawn in auction.");
			/*GameObject newPlayer = Instantiate(arenaPrefab);
			Robot robot = newPlayer.GetComponent<Robot>();
			robot.playerGO = gameObject;*/

			upgradeAssigned = false;
			GameObject newPlayer = Instantiate(auctionPrefab);
			AuctionPlayer pb = newPlayer.GetComponent<AuctionPlayer>();
			pb.playerGO = gameObject;
			NetworkServer.Spawn(newPlayer);
			if (!isAgent)
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);

			GameObject playerScraps = Instantiate(auctionPlayerScraps);
			PlayerScraps ps = playerScraps.GetComponent<PlayerScraps>();
			ps.playerBoxGO = pb.gameObject;
			NetworkServer.Spawn(playerScraps);

			if (!FindObjectOfType<NetworkAuctionManager>()) {
				GameObject NAM = Instantiate(networkAuctionManager);
				NetworkServer.Spawn(NAM);
			}
		} /*else if (SceneManager.GetActiveScene().name == GameScenes.MatchResult) {
			GameObject newPlayer = Instantiate(matchResultPrefab);
			PlayerResult res = newPlayer.GetComponent<PlayerResult>();
			res.playerGO = gameObject;
			NetworkServer.Spawn(newPlayer);
		}*/
	}

	[Command]
	public void CmdAddPermanentUpgrade(int level, int ID) {
		upgradeAssigned = true;
		AddPermanentUpgrade(level, ID);
		RpcAddPermanentUpgrade(level, ID);
	}
	[ClientRpc]
	void RpcAddPermanentUpgrade(int level, int ID) {
		AddPermanentUpgrade(level, ID);
	}
	void AddPermanentUpgrade(int level, int ID) {
		if (upgrades[(int)Upgrades.permanent[level][ID].type]) {
			RemoveUpgrade((int)Upgrades.permanent[level][ID].type);
		}
		upgrades[(int)Upgrades.permanent[level][ID].type] = new Pair(level, ID);
		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			pb.ShowUpgrade((int)Upgrades.permanent[level][ID].type);
		}
	}

	void RemoveUpgrade(int type) {
		if (robot) {
			Upgrades.permanent[upgrades[type].value1][upgrades[type].value2].OnRemove(robot);
		}
		upgrades[type] = null;
	}

	[Command]
	public void CmdSetupAuctionAgent() {
		/*if (upgradesBalance == UpgradesBalance.notSet) {
			upgradesBalance = (UpgradesBalance)Random.Range(1, System.Enum.GetValues(typeof(UpgradesBalance)).Length);
		}*/
		if (upgradesPrefer == UpgradesPrefer.notSet) {
			upgradesPrefer = (UpgradesPrefer)Random.Range(1, System.Enum.GetValues(typeof(UpgradesPrefer)).Length);
		}
		RpcSetupAuctionAgent(upgradesBalance, upgradesPrefer);
	}
	[ClientRpc]
	public void RpcSetupAuctionAgent(UpgradesBalance upgradesBalance, UpgradesPrefer upgradesPrefer) {
		this.upgradesBalance = upgradesBalance;
		this.upgradesPrefer = upgradesPrefer;
	}

	private void OnDisconnectedFromServer(NetworkIdentity info) {
		Destroy(gameObject);
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Debug.Log("Scene loaded: " + scene.name);
		if (transform.childCount > 0) {
			Debug.Log("Destroy child");
			Destroy(transform.GetChild(0).gameObject);
		}
		CmdRespawn(gameObject);
	}
}
