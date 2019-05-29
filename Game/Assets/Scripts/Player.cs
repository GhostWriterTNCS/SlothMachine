using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {
	[SyncVar]
	public int playerID;
	[SyncVar]
	public string robotName;
	[SyncVar]
	public int score;
	[SyncVar]
	public int scraps;
	[SyncVar]
	public int roundWinner;
	[SyncVar]
	public bool isAgent;
	/*[SyncVar]
	public int roundCounter;*/

	public GameObject auctionPrefab;
	public GameObject auctionPlayerScraps;
	public GameObject networkAuctionManager;
	public GameObject arenaPrefab;
	public GameObject networkArenaManager;

	public List<Pair> upgrades = new List<Pair>();
	[SyncVar]
	public bool upgradeAssigned;

	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		name = "Player " + playerID + (isAgent ? " (bot)" : "");
		score = 0;
		scraps = 100;
		roundWinner = 0;
		//roundCounter = 0;
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
			NetworkServer.Spawn(newPlayer);
			Robot robot = newPlayer.GetComponent<Robot>();
			robot.playerGO = gameObject;
			robot.roundScore = 0;
			if (!isAgent) {
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
				FindObjectOfType<ArenaManager>().upgradeWheel.player = this;
			}

			if (!FindObjectOfType<NetworkArenaManager>()) {
				GameObject NAM = Instantiate(networkArenaManager);
				NetworkServer.Spawn(NAM);
				NAM.GetComponent<NetworkArenaManager>().CmdLoad();
			}
		} else if (SceneManager.GetActiveScene().name == GameScenes.Auction) {
			Debug.Log("Spawn in auction.");
			upgradeAssigned = false;
			GameObject newPlayer = Instantiate(auctionPrefab);
			NetworkServer.Spawn(newPlayer);
			AuctionPlayer pb = newPlayer.GetComponent<AuctionPlayer>();
			pb.playerGO = gameObject;
			if (!isAgent)
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);

			GameObject playerScraps = Instantiate(auctionPlayerScraps);
			NetworkServer.Spawn(playerScraps);
			PlayerScraps ps = playerScraps.GetComponent<PlayerScraps>();
			ps.playerBoxGO = pb.gameObject;
			//RpcInitPlayerScraps(playerScraps);

			if (!FindObjectOfType<NetworkAuctionManager>()) {
				GameObject NAM = Instantiate(networkAuctionManager);
				NetworkServer.Spawn(NAM);
				NAM.GetComponent<NetworkAuctionManager>().CmdLoad();
			}
		}
	}

	[Server]
	public void IncreaseRoundCounter() {
		FindObjectOfType<MatchManager>().roundCounter++;
	}

	[Command]
	public void CmdAddPermanentUpgrade(int level, int ID) {
		upgradeAssigned = true;
		RpcAddPermanentUpgrade(level, ID);
	}
	[Command]
	public void CmdAddTemporaryUpgrade(int ID) {
		Upgrade u = Upgrades.temporary[ID];
		if (u.price <= scraps) {
			scraps -= u.price;
			u.OnAdd(GetComponentInChildren<Robot>());
			Debug.Log("Upgrade " + u.name + " mounted!");
		} else {
			Debug.Log("Not enough scraps!");
		}
	}
	[ClientRpc]
	public void RpcAddPermanentUpgrade(int level, int ID) {
		upgrades.Add(new Pair(level, ID));
		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			pb.ShowUpgrade(upgrades.Count - 1);
		}
	}

	[Command]
	public void CmdRemoveUpgrade(int level, int ID) {
		if (upgrades.Contains(new Pair(level, ID))) {
			upgrades.Remove(new Pair(level, ID));
		}
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
