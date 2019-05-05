using UnityEngine;
using UnityEngine.Networking;

public class EscToStop : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (NetworkServer.active) {
				NetworkManager.singleton.StopHost();
			} else if (NetworkClient.active) {
				NetworkManager.singleton.StopClient();
			}
		}
	}
}
