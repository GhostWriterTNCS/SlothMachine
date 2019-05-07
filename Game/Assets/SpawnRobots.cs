using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnRobots : NetworkBehaviour {

	void Start() {
		if (isServer) {
			Debug.Log("Wait for Spawn");
			StartCoroutine(WaitForSpawn());
		}
	}

	IEnumerator WaitForSpawn() {
		while (true) {
			if (isServer) {
				if (connectionToServer != null && !connectionToServer.isReady) {
					yield return new WaitForSeconds(0.25f);
				} else {
					break;
				}
			} else {
				if (connectionToClient != null && !connectionToClient.isReady) {
					yield return new WaitForSeconds(0.25f);
				} else {
					break;
				}
			}
		}
		CmdSpawn();
	}

	[Command]
	void CmdSpawn() {
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (p.robot == null) {
				GameObject robot = Instantiate(NetworkManager.singleton.spawnPrefabs[1], NetworkManager.singleton.GetStartPosition().position, NetworkManager.singleton.GetStartPosition().rotation);
				NetworkServer.SpawnWithClientAuthority(robot, p.gameObject);
				robot.name = "Avatar " + p.playerControllerId;
				robot.GetComponent<Robot>().player = p.gameObject;
				NetworkServer.ReplacePlayerForConnection(p.connectionToClient, robot, p.playerControllerId);
				p.robot = robot;
			}
		}
	}
}
