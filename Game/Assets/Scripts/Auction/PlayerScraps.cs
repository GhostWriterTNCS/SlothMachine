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
	public PlayerBox playerBox;

	[SyncVar]
	public GameObject highestBid;

	void Start() {
		StartCoroutine(LoadPlayer());
	}

	IEnumerator LoadPlayer() {
		while (!playerBoxGO) {
			yield return new WaitForSeconds(0.01f);
		}
		transform.SetParent(FindObjectOfType<AuctionManager>().scrapsList.transform);
		playerBox = playerBoxGO.GetComponent<PlayerBox>();
		playerName.text = playerBox.player.name;
	}

	public void UpdateResult() {
		if (playerBox.player.isAgent) {
			CmdCalculateAgentsBids();
		}
		StartCoroutine(UpdateResultDelay());
	}

	[Command]
	public void CmdCalculateAgentsBids() {
		playerBox.bid = Random.Range(0, playerBox.player.scraps + 1);
		playerBox.bidRegistered = true;
	}

	IEnumerator UpdateResultDelay() {
		float maxValue = 0;
		foreach (PlayerBox pb in FindObjectsOfType<PlayerBox>()) {
			while (!pb.bidRegistered) {
				yield return new WaitForSeconds(0.01f);
			}
			if (pb.bid > maxValue) {
				maxValue = pb.bid;
				highestBid = pb.gameObject;
			}
		}

		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			if (ps.playerBoxGO == highestBid) {
				maxValue = ps.playerBox.bid;
				ps.playerName.fontStyle = FontStyle.Bold;
				ps.scrapsValue.fontStyle = FontStyle.Bold;
			} else {
				ps.playerName.fontStyle = FontStyle.Normal;
				ps.scrapsValue.fontStyle = FontStyle.Normal;
			}
		}
		scrapsSlider.value = playerBox.bid / maxValue;
		scrapsValue.text = playerBox.bid.ToString();
	}
}
