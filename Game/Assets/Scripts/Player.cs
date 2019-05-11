﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {
	[SyncVar, HideInInspector]
	public int playerID;
	[SyncVar, HideInInspector]
	public string robotName;
	[SyncVar, HideInInspector]
	public int score;
	[SyncVar, HideInInspector]
	public int scraps;

	public GameObject auctionPrefab;
	public GameObject auctionPlayerScraps;
	public GameObject arenaPrefab;

	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		name = "Player " + playerID;
		CmdRespawn(gameObject);
		score = 0;
		scraps = 100;
	}

	[Command]
	public void CmdRespawn(GameObject go) {
		NetworkConnection conn = go.GetComponent<NetworkIdentity>().connectionToClient;
		NetworkServer.ReplacePlayerForConnection(conn, gameObject, 0);
		if (SceneManager.GetActiveScene().name == GameScenes.Arena) {
			Debug.Log("Spawn in arena.");
			GameObject newPlayer = Instantiate(arenaPrefab, transform);
			NetworkServer.Spawn(newPlayer);
			RpcSetParent(newPlayer, gameObject);
			NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
		} else if (SceneManager.GetActiveScene().name == GameScenes.Auction) {
			Debug.Log("Spawn in auction.");
			GameObject newPlayer = Instantiate(auctionPrefab);
			NetworkServer.Spawn(newPlayer);
			PlayerBox pb = newPlayer.GetComponent<PlayerBox>();
			pb.playerGO = gameObject;
			NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);

			GameObject playerScraps = Instantiate(auctionPlayerScraps);
			NetworkServer.Spawn(playerScraps);
			PlayerScraps ps = playerScraps.GetComponent<PlayerScraps>();
			ps.playerBoxGO = pb.gameObject;

			//NetworkServer.ReplacePlayerForConnection(conn, FindObjectOfType<ScrapsInput>().gameObject, 0);

			/*GameObject NAM = Instantiate(new GameObject());
			NAM.AddComponent<NetworkAuctionManager>();
			NetworkServer.Spawn(NAM);*/
		}
	}
	[ClientRpc]
	public void RpcSetParent(GameObject go, GameObject parent) {
		Debug.Log("Set parent: " + go.name + " - " + parent.name);
		go.transform.SetParent(parent.transform);
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