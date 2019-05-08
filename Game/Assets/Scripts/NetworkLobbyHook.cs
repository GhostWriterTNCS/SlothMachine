using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		//base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		Robot r = gamePlayer.GetComponent<Robot>();

		r.robotName = lobby.playerName;
	}
}
