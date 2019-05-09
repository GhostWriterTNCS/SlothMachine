﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PopulateRobotsGrid : NetworkBehaviour {
	public Prototype.NetworkLobby.LobbyPlayer lobbyPlayer;

	public IEnumerator LoadDefault() {
		while (lobbyPlayer == null) {
			yield return new WaitForSeconds(0.01f);
		}
		LoadKiddo();
	}

	public void LoadKiddo() {
		lobbyPlayer.CmdNameChanged("Kiddo");
	}

	public void LoadNourinha() {
		lobbyPlayer.CmdNameChanged("Nourinha");
	}

	public void LoadBreach() {
		lobbyPlayer.CmdNameChanged("Breach");
	}

	public void LoadDozzer() {
		lobbyPlayer.CmdNameChanged("Dozzer");
	}
}
