using UnityEngine;
using UnityEngine.UI;

public class AuctionManager : MonoBehaviour {
	public Text header;
	public GameObject playersList;
	public GameObject scrapsInput;
	public GameObject scrapsWait;
	public GameObject scrapsList;
	public GameObject upgradesList;
	public UpgradeBox currentUpgrade;
	public NetworkAuctionManager networkAuctionManager;

	public void CalculateAgentsBids() {
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			if (ps.playerBox.player.isAgent && !ps.playerBox.player.upgradeAssigned) {
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
