using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {
	[SyncVar]
	public string playerID = "";
	[SyncVar]
	public string robotName = "Dozzer";
	[SyncVar]
	public int score = 0;

	public GameObject avatarPrefab;

	private void Awake() {
		Debug.Log("Player awake.");
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		Debug.Log("Player start.");
		name = playerID;
		CmdRespawn(gameObject);
	}

	[Command]
	public void CmdRespawn(GameObject go) {
		NetworkConnection conn = go.GetComponent<NetworkIdentity>().connectionToClient;
		NetworkServer.ReplacePlayerForConnection(conn, gameObject, 0);
		if (SceneManager.GetActiveScene().name == GameScenes.Arena) {
			Debug.Log("Spawn in arena.");
			GameObject newPlayer = Instantiate(avatarPrefab, transform);
			NetworkServer.Spawn(newPlayer);
			RpcSetParent(newPlayer, gameObject);
			NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
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
