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

	public void EvaluateBids() {
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			ps.UpdateResult();
		}
	}
}
