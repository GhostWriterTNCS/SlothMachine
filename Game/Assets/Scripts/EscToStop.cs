using UnityEngine;
using UnityEngine.Networking;

public class EscToStop : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape) && (NetworkServer.active || NetworkClient.active)) {
			NetworkManager.singleton.StopHost();
		}
	}
}
