using System.Collections;
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

	void Start() {
		backgroundImage.gameObject.SetActive(false);
		FindObjectOfType<AuctionManager>().StartCoroutine(LoadPlayer());
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
		auctionPlayer.bid = (short)Random.Range(0, auctionPlayer.player.scraps + 1);
		auctionPlayer.bidRegistered = true;
	}

	IEnumerator UpdateResultCoroutine() {
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
	}
}
