using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaBuilder : MonoBehaviour {
	static ArenaBuilder singleton;

	void Awake() {
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
