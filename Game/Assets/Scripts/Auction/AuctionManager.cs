using UnityEngine;

public class AuctionManager : MonoBehaviour {
	public GameObject playersList;
	public GameObject scrapsList;
	public GameObject upgradesList;

	//[SyncVar]
	//public GameObject highestBid;

	public void EvaluateBids() {
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			ps.UpdateResult();
		}
	}
}
