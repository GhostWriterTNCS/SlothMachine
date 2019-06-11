using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		//base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		Player p = gamePlayer.GetComponent<Player>();
        p.playerID = 1;// lobby.playerID;
		p.robotName = lobby.playerName;
        p.isAgent = false;// lobby.isAgent;
		p.color = lobby.playerColor;
	}
}
