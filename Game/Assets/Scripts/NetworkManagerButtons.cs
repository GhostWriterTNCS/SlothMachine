using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManagerButtons : MonoBehaviour {
	public void StartHost() {
		SceneManager.LoadScene(GameScenes.Lobby);
		//NetworkManager.singleton.StartHost();
	}

	public void StartClient() {
		NetworkManager.singleton.StartClient();
	}

	public void Stop() {
		NetworkManager.singleton.StopHost();
	}
}
