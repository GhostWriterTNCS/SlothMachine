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

	AuctionAgent auctionAgent;
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
		auctionAgent = new AuctionAgentRobot(auctionPlayer.player);
	}

	[Command]
	public void CmdCalculateAgentsBids() {
		currentUpgrade = auctionManager.currentUpgrade;
		auctionAgent.variability = variability;
		auctionAgent.moneyAvailable = auctionPlayer.player.scraps;
		List<Player> players = new List<Player>();
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (!p.upgradeAssigned && p != auctionPlayer.player) {
				players.Add(p);
			}
		}
		auctionPlayer.bid = (short)auctionAgent.GetRefinedBid(currentUpgrade, auctionPlayer.player, players.ToArray());
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
