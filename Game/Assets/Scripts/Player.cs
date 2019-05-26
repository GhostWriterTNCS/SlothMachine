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
	[SyncVar]
	public bool upgradeAssigned;

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
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			Debug.Log("Spawn in arena.");
			GameObject newPlayer = Instantiate(arenaPrefab);
			Transform t = NetworkManager.singleton.GetStartPosition();
			newPlayer.transform.position = t.position;
			newPlayer.transform.rotation = t.rotation;
			NetworkServer.Spawn(newPlayer);
			newPlayer.GetComponent<Robot>().playerGO = gameObject;
			if (!isAgent) {
				NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
				FindObjectOfType<ArenaManager>().upgradeWheel.player = this;
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

			if (FindObjectOfType<NetworkAuctionManager>() == null) {
				GameObject NAM = Instantiate(networkAuctionManager);
				NetworkServer.Spawn(NAM);
				NAM.GetComponent<NetworkAuctionManager>().CmdLoad();
			}
		}
	}

	/*[ClientRpc]
	public void RpcInitPlayerScraps(GameObject ps) {
		StartCoroutine(ps.GetComponent<PlayerScraps>().LoadPlayer());
	}*/

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


	// -- NetworkAuctionManager --


}
