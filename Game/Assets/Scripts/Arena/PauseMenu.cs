using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour {
	public Robot robot;
	public Button focusButton;

	void Update() {
		if (Input.GetButtonUp("Menu")) {
			Pause();
		}
	}

	public void Pause() {
		if (!robot) {
			return;
		}
		gameObject.SetActive(!robot.paused);
		robot.paused = !robot.paused;
		if (robot.paused) {
			FindObjectOfType<EventSystem>().SetSelectedGameObject(focusButton.gameObject);
			Debug.Log("Focus");
		}
	}

	public void Leave() {
		try {
			Prototype.NetworkLobby.LobbyManager.s_Singleton.StopClientClbk();
			Prototype.NetworkLobby.LobbyManager.s_Singleton.StopHostClbk();
		} catch (Exception e) {
			Debug.LogError(e);
		}
		SceneManager.LoadScene(GameScenes.StartScreen);
		//StartCoroutine(LeaveCoroutine());
	}

	IEnumerator LeaveCoroutine() {
		while (FindObjectOfType<Prototype.NetworkLobby.LobbyManager>()) {
			try {
				Prototype.NetworkLobby.LobbyManager.s_Singleton.StopClientClbk();
				Prototype.NetworkLobby.LobbyManager.s_Singleton.StopHostClbk();
				SceneManager.LoadScene(GameScenes.StartScreen);
				//Destroy(FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().gameObject);
			} catch (Exception e) {
				//Debug.LogError(e);
			}
			yield return 0;
		}
	}
}
