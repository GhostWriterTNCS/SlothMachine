using UnityEngine;
using UnityEngine.Networking;

public class ArenaManager : MonoBehaviour {
	public GameObject leaderboard;
	public UpgradeWheel upgradeWheel;
	public GameObject arenaBoxPrefab;

	public void RoundOver() {
		NetworkManager.singleton.ServerChangeScene(GameScenes.Auction);
	}
}
