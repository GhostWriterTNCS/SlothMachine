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

	void Start() {
		StartCoroutine(CreateUpgradeBoxesCoroutine());
	}
	IEnumerator CreateUpgradeBoxesCoroutine() {
		while (!networkAuctionManager) {
			yield return 0;
		}
		while (networkAuctionManager.auctionUpgrades.Count < MatchManager.singleton.playerCount * 2) {
			yield return 0;
		}
		for (int i = 0; i < MatchManager.singleton.playerCount; i++) {
			byte level = (byte)networkAuctionManager.auctionUpgrades[i * 2];
			byte upgrade = (byte)networkAuctionManager.auctionUpgrades[i * 2 + 1];

			GameObject upgradeBox = Instantiate(networkAuctionManager.upgradeBoxPrefab);
			UpgradeBox ub = upgradeBox.GetComponent<UpgradeBox>();
			ub.level = level;
			ub.ID = upgrade;
			ub.selected = (i == 0);
			if (networkAuctionManager.isServer) {
				networkAuctionManager.upgrades.Add(ub);
			}

			GameObject upgradeBoxWithDesc = Instantiate(networkAuctionManager.upgradeBoxWithDescPrefab);
			ub = upgradeBoxWithDesc.GetComponent<UpgradeBox>();
			ub.level = level;
			ub.ID = upgrade;
			ub.isIntro = true;
		}
	}

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
