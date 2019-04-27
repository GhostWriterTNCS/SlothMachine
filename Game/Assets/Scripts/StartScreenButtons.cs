using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenButtons : MonoBehaviour {
	public void Play() {
		SceneManager.LoadScene(GameScenes.MatchType);
	}

	public void Leaderboards() {
		SceneManager.LoadScene(GameScenes.Leaderboards);
	}

	public void Customize() {
		SceneManager.LoadScene(GameScenes.Customize);
	}

	public void Shop() {
		SceneManager.LoadScene(GameScenes.Shop);
	}

	public void Options() {

	}

	public void InviteFriends() {

	}

	public void Quit() {
		Application.Quit();
	}
}
