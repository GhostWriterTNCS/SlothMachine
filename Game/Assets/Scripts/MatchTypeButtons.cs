using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchTypeButtons : MonoBehaviour {
	public void ArenaOnline() {
		SceneManager.LoadScene(GameScenes.AvatarScreen);
	}

	public void ArenaOffline() {
		SceneManager.LoadScene(GameScenes.AvatarScreen);
	}

	public void Training() {
		SceneManager.LoadScene(GameScenes.AvatarScreen);
	}

	public void Back() {
		SceneManager.LoadScene(GameScenes.StartScreen);
	}
}
