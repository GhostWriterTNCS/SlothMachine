using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerScraps : NetworkBehaviour {
	public Text playerName;
	public Slider scrapsSlider;
	public Text scrapsValue;

	[SyncVar]
	public GameObject playerBoxGO;
	public AuctionPlayer playerBox;

	void Start() {
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
		playerBox = playerBoxGO.GetComponent<AuctionPlayer>();
		playerName.text = playerBox.player.name;
	}

	[Command]
	public void CmdCalculateAgentsBids() {
		playerBox.bid = Random.Range(0, playerBox.player.scraps + 1);
		playerBox.bidRegistered = true;
	}

	IEnumerator UpdateResultCoroutine() {
		NetworkAuctionManager NAM = FindObjectOfType<NetworkAuctionManager>();
		while (NAM.auctionWinner == null) {
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
			ps.scrapsSlider.value = ps.playerBox.bid / (float)NAM.maxBid;
			ps.scrapsValue.text = ps.playerBox.bid.ToString();
		}
	}
}
