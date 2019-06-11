using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RefreshScene : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.F5)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
