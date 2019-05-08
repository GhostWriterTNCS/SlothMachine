using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnRobots : NetworkBehaviour {

	void Start() {
		if (isServer) {
			Debug.Log("Wait for Spawn " + NetworkServer.connections.Count + " players.");
			//CmdSpawn();
			StartCoroutine(WaitForSpawn());
		}
	}

	IEnumerator WaitForSpawn() {
		if (NetworkServer.connections.Count > 1) {
			while (connectionToClient == null || !connectionToClient.isReady) {
				yield return new WaitForSeconds(0.25f);
			}
		}
		CmdSpawn();
	}

	[Command]
	void CmdSpawn() {
		Debug.Log("Spawn robots");
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (p.robot == null) {
				Transform t = NetworkManager.singleton.GetStartPosition();
				GameObject robot = Instantiate(NetworkManager.singleton.spawnPrefabs[1], t.position, t.rotation);
				NetworkServer.SpawnWithClientAuthority(robot, p.gameObject);
				robot.name = "Avatar " + p.playerControllerId;
				robot.GetComponent<Robot>().player = p.gameObject;
				NetworkServer.ReplacePlayerForConnection(p.connectionToClient, robot, p.playerControllerId);
				p.robot = robot;
			}
		}
	}
}
