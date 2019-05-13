using UnityEngine;
using UnityEngine.Networking;

public class LoadArena : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		}
	}
}
