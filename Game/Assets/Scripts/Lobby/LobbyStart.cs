using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyStart : MonoBehaviour {
	void Start() {
		MatchManager.singleton.playerCount = 0;
		StartCoroutine(Select());
	}

	IEnumerator Select() {
		FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
		yield return 0;
		FindObjectOfType<EventSystem>().SetSelectedGameObject(FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().mainPanelFirstButton);
		FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().mainPanelFirstButton.GetComponent<Button>().OnSelect(null);
	}
}
