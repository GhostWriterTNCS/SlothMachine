using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AuctionManager : MonoBehaviour {
	public Text header;
	public GameObject introPanel;
	public GameObject auctionPanel;
	public GameObject playersList;
	public GameObject scrapsInput;
	public GameObject scrapsWait;
	public GameObject scrapsList;
	public GameObject upgradesList;
	public UpgradeBox currentUpgrade;
	public NetworkAuctionManager networkAuctionManager;

	void Update() {
		if (networkAuctionManager) {
			header.text = networkAuctionManager.currentTitle;
		}
	}

	public void CalculateAgentsBids() {
		foreach (PlayerScraps ps in Resources.FindObjectsOfTypeAll<PlayerScraps>()) {
			if (ps.auctionPlayer && ps.auctionPlayer.player.isAgent && !ps.auctionPlayer.player.upgradeAssigned) {
				ps.CmdCalculateAgentsBids();
			}
		}
	}

	public void UpdateResults() {
		StartCoroutine(UpdateResultsCoroutine());
	}

	IEnumerator UpdateResultsCoroutine() {
		while (!networkAuctionManager.auctionRegistered) {
			yield return 0;
		}
		foreach (PlayerScraps ps in Resources.FindObjectsOfTypeAll<PlayerScraps>()) {
			if (ps.auctionPlayer) {
				if (ps.playerBoxGO == networkAuctionManager.auctionWinner) {
					ps.playerName.fontStyle = FontStyle.Bold;
					ps.scrapsValue.fontStyle = FontStyle.Bold;
				} else {
					ps.playerName.fontStyle = FontStyle.Normal;
					ps.scrapsValue.fontStyle = FontStyle.Normal;
				}
				ps.scrapsSlider.value = ps.auctionPlayer.bid / (float)networkAuctionManager.maxBid;
				ps.scrapsValue.text = ps.auctionPlayer.bid.ToString();
			}
		}
	}
}
