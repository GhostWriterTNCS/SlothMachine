using UnityEngine;
using UnityEngine.Networking;

public class AvatarScreenButtons : MonoBehaviour {
	public void Back() {
		if (NetworkServer.active) {
			NetworkManager.singleton.StopHost();
		} else if (NetworkClient.active) {
			NetworkManager.singleton.StopClient();
		}
	}

	public void Confirm() {
		NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
	}
}
