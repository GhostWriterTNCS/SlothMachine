using UnityEngine;
using UnityEngine.Networking;

public class ArenaManager : MonoBehaviour {
	public void RoundOver() {
		NetworkManager.singleton.ServerChangeScene(GameScenes.Auction);
	}
}
