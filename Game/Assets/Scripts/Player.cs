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
	public bool isAgent;

	public GameObject auctionPrefab;
	public GameObject auctionPlayerScraps;
	public GameObject networkAuctionManager;
	public GameObject arenaPrefab;

	public List<Pair> upgrades = new List<Pair>();

	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		if (isAgent) {
			name = "Bot " + playerID;
		} else {
			name = "Player " + playerID;
		}
		CmdRespawn(gameObject);
		score = 0;
		scraps = 100;
	}

	[Command]
	public void CmdRespawn(GameObject go) {
		NetworkConnection conn = go.GetComponent<NetworkIdentity>().connectionToClient;
		if (!isAgent)
			NetworkServer.ReplacePlayerForConnection(conn, gameObject, 0);
		if (SceneManager.GetActiveScene().name == GameScenes.Arena) {
			Debug.Log("Spawn in arena.");
			GameObject newPlayer = Instantiate(arenaPrefab, transform);
			NetworkServer.Spawn(newPlayer);
			RpcSetParent(newPlayer, gameObject);
			if (!isAgent)
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
		} else if (SceneManager.GetActiveScene().name == GameScenes.Auction) {
			Debug.Log("Spawn in auction.");
			GameObject newPlayer = Instantiate(auctionPrefab);
			NetworkServer.Spawn(newPlayer);
			PlayerBox pb = newPlayer.GetComponent<PlayerBox>();
			pb.playerGO = gameObject;
			if (!isAgent)
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);

			GameObject playerScraps = Instantiate(auctionPlayerScraps);
			NetworkServer.Spawn(playerScraps);
			PlayerScraps ps = playerScraps.GetComponent<PlayerScraps>();
			ps.playerBoxGO = pb.gameObject;

			if (FindObjectOfType<NetworkAuctionManager>() == null) {
				GameObject NAM = Instantiate(networkAuctionManager);
				NetworkServer.Spawn(NAM);
				NAM.GetComponent<NetworkAuctionManager>().CmdLoad();
			}
		}
	}
	[ClientRpc]
	public void RpcSetParent(GameObject go, GameObject parent) {
		Debug.Log("Set parent: " + go.name + " - " + parent.name);
		go.transform.SetParent(parent.transform);
	}

	[Command]
	public void CmdAddUpgrade(int level, int ID) {
		if (upgrades.Contains(new Pair(level, ID))) {
			upgrades.Add(new Pair(level, ID));
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


	// -- NetworkAuctionManager --


}
