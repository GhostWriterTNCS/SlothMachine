﻿using UnityEngine;
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
		DestroyImmediate(FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().gameObject);
		SceneManager.LoadScene(GameScenes.StartScreen);
	}
}
