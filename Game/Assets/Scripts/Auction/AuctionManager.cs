using UnityEngine;
using UnityEngine.UI;

public class AuctionManager : MonoBehaviour {
	public Text header;
	public GameObject playersList;
	public GameObject scrapsList;
	public GameObject upgradesList;
	public UpgradeBox currentUpgrade;
	public NetworkAuctionManager networkAuctionManager;

	//[SyncVar]
	//public GameObject highestBid;

	public void CalculateAgentsBids() {
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			if (ps.playerBox.player.isAgent) {
				ps.CmdCalculateAgentsBids();
			}
		}
	}

	public void UpdateResults() {
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			ps.UpdateResult();
		}
	}
}
