using UnityEngine;

public class AuctionManager : MonoBehaviour {
	public GameObject playersList;
	public GameObject scrapsList;
	public GameObject upgradesList;

	public void EvaluateBids() {
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			ps.UpdateResult();
		}
	}
}
