using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerButtons : MonoBehaviour {
	public void StartHost() {
		NetworkManager.singleton.StartHost();
	}

	public void StartClient() {
		NetworkManager.singleton.StartClient();
	}

	public void Stop() {
		NetworkManager.singleton.StopHost();
	}
}