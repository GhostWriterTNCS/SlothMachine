using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MatchManager : NetworkBehaviour {
	public static MatchManager singleton;
	public int playerCount;
	public int roundCounter;
	public bool bossRound;
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
		playerCount = 0;
		roundCounter = 0;
		bossRound = false;
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
