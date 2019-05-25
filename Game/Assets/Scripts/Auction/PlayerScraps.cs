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

	void Awake() {
		transform.SetParent(FindObjectOfType<AuctionManager>().scrapsList.transform);
		FindObjectOfType<AuctionManager>().StartCoroutine(LoadPlayer());
	}

	public IEnumerator LoadPlayer() {
		Debug.Log("Awaken");
		while (!playerBoxGO) {
			Debug.Log("Searching playerBoxGO");
			yield return new WaitForSeconds(0.05f);
		}
		Debug.Log("playerBoxGO found");
		playerBox = playerBoxGO.GetComponent<AuctionPlayer>();
		playerName.text = playerBox.player.name;
	}

	public void UpdateResult() {
		//FindObjectOfType<AuctionManager>().StartCoroutine(UpdateResultCoroutine());
	}

	[Command]
	public void CmdCalculateAgentsBids() {
		playerBox.bid = Random.Range(0, playerBox.player.scraps + 1);
		playerBox.bidRegistered = true;
	}

	IEnumerator UpdateResultCoroutine() {
		NetworkAuctionManager NAM = FindObjectOfType<NetworkAuctionManager>();
		while (NAM.auctionWinner == null) {
			yield return new WaitForSeconds(0.05f);
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
