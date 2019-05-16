using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MatchTypeButtons : MonoBehaviour {
	public void ArenaOnline() {
		Debug.Log("ArenaOnline");
		SceneManager.LoadScene(GameScenes.AvatarScreen);
	}

	public void ArenaOffline() {
		Debug.Log("ArenaOffline");
		SceneManager.LoadScene(GameScenes.AvatarScreen);
	}

	public void Training() {
		Debug.Log("Training");
		SceneManager.LoadScene(GameScenes.AvatarScreen);
	}

	public void Back() {
		Debug.Log("Back");
		NetworkLobbyManager.singleton.StopHost();
		Destroy(FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().gameObject);
		SceneManager.LoadScene(GameScenes.StartScreen);
	}
}
