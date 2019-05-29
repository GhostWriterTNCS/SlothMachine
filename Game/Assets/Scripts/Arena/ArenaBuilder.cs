using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaBuilder : MonoBehaviour {
	public static ArenaBuilder singleton;
	public bool arenaReady;

	void Awake() {
		arenaReady = false;
		if (!singleton || SceneManager.GetActiveScene().name == GameScenes.Arena) {
			if (singleton) {
				Destroy(singleton.gameObject);
			}
			singleton = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}
}
