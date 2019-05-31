﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MatchManager : NetworkBehaviour {
	public static MatchManager singleton;
	//[SyncVar]
	public int roundCounter;
	bool handlerAdded;

	void Awake() {
		if (singleton == null) {
			singleton = this;
		} else if (singleton != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		roundCounter = 0;
	}

	void OnEnable() {
		if (!handlerAdded) {
			SceneManager.sceneLoaded += OnSceneLoaded;
			handlerAdded = true;
		}
	}
	void OnDisable() {
		//SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name == GameScenes.Arena && roundCounter >= 0) {
			roundCounter++;
		} else if (scene.name == GameScenes.StartScreen) {
			SceneManager.sceneLoaded -= OnSceneLoaded;
			Destroy(gameObject);
		}
	}
}