using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerScraps : NetworkBehaviour {
	public Text playerName;
	public Slider scrapsSlider;
	public Text scrapsValue;
	public Image backgroundImage;

	[SyncVar]
	public GameObject playerBoxGO;
	public AuctionPlayer auctionPlayer;

	[Header("Agent")]
	[Range(0, 1)]
	public float variability = 0.1f;

	AuctionManager auctionManager;
	UpgradeBox currentUpgrade;

	void Start() {
		backgroundImage.gameObject.SetActive(false);
		auctionManager = FindObjectOfType<AuctionManager>();
		auctionManager.StartCoroutine(LoadPlayer());
	}

	public IEnumerator LoadPlayer() {
		while (!playerBoxGO) {
			yield return 0;
		}
		while (FindObjectOfType<Canvas>().transform.localScale.x < 0.1f) {
			yield return 0;
		}
		transform.localScale = FindObjectOfType<Canvas>().transform.localScale;
		transform.SetParent(FindObjectOfType<AuctionManager>().scrapsList.transform);
		auctionPlayer = playerBoxGO.GetComponent<AuctionPlayer>();
		while (!auctionPlayer.player) {
			yield return 0;
		}
		playerName.text = auctionPlayer.player.name;
		if (auctionPlayer.isLocalPlayer && !auctionPlayer.player.isAgent) {
			backgroundImage.gameObject.SetActive(true);
			backgroundImage.color = TextManager.backgroundHighlightedColor;
		}
	}

	[Command]
	public void CmdCalculateAgentsBids() {
		currentUpgrade = auctionManager.currentUpgrade;
		auctionPlayer.auctionAgent.variability = variability;
		auctionPlayer.auctionAgent.moneyAvailable = auctionPlayer.player.scraps;
		auctionPlayer.auctionAgent.balanceWeight = 2;
		auctionPlayer.auctionAgent.preferWeight = 2;

		List<Player> players = new List<Player>();
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (!p.upgradeAssigned && p != auctionPlayer.player) {
				players.Add(p);
			}
		}

		float bid = auctionPlayer.auctionAgent.GetRefinedBid(currentUpgrade, auctionPlayer.player, players.ToArray());
		if (auctionPlayer.upgradeInterests.Count == 0) {
			auctionPlayer.CmdEvaluateUpgrades();
		}
		if (auctionPlayer.upgradeInterests[0].upgradeBox == currentUpgrade) {
			if (auctionPlayer.upgradeInterests.Count > 3) {
				bid *= 1.3f;
			} else if (auctionPlayer.upgradeInterests.Count > 2) {
				bid *= 1.2f;
			} else {
				bid *= 1.1f;
			}
		} else if (auctionPlayer.upgradeInterests.Count > 3 && auctionPlayer.upgradeInterests[3].upgradeBox == currentUpgrade) {
			bid *= 0.5f;
		} else if (auctionPlayer.upgradeInterests.Count > 2 && auctionPlayer.upgradeInterests[2].upgradeBox == currentUpgrade) {
			bid *= 0.8f;
		}
		if (bid > auctionPlayer.player.scraps) {
			bid = auctionPlayer.player.scraps;
		}
		auctionPlayer.bid = (short)bid;
		auctionPlayer.bidRegistered = true;
	}

	/*IEnumerator UpdateResultCoroutine() {
		NetworkAuctionManager NAM = FindObjectOfType<NetworkAuctionManager>();
		while (!NAM.auctionRegistered) {
			yield return 0;
		}
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			if (ps.playerBoxGO == NAM.auctionWinner) {
				ps.playerName.fontStyle = FontStyle.Bold;
				ps.scrapsValue.fontStyle = FontStyle.Bold;
			} else {
				ps.playerName.fontStyle = FontStyle.Normal;
				ps.scrapsValue.fontStyle = FontStyle.Normal;
			}
			ps.scrapsSlider.value = ps.auctionPlayer.bid / (float)NAM.maxBid;
			ps.scrapsValue.text = ps.auctionPlayer.bid.ToString();
		}
	}*/
}
