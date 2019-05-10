using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		//base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		/*Robot r = gamePlayer.GetComponent<Robot>();
		if (r) {
			r.robotName = lobby.playerName;
		} else {*/
		RpcRename(gamePlayer, lobby.playerID);
		gamePlayer.GetComponent<Player>().robotName = lobby.playerName;
		//}
	}

	//[ClientRpc]
	public void RpcRename(GameObject go, string name) {
		go.name = name;
	}
}
