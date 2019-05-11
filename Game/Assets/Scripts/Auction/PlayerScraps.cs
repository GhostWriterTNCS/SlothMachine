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
		StartCoroutine(UpdateResultDelay());
	}

	IEnumerator UpdateResultDelay() {
		foreach (PlayerBox pb in FindObjectsOfType<PlayerBox>()) {
			while (!pb.bidRegistered) {
				yield return new WaitForSeconds(0.01f);
			}
		}

		float maxValue = 0;
		foreach (PlayerScraps ps in FindObjectsOfType<PlayerScraps>()) {
			if (ps.playerBox.bid > maxValue) {
				maxValue = ps.playerBox.bid;
			}
		}
		scrapsSlider.value = playerBox.bid / maxValue;
		scrapsValue.text = playerBox.bid.ToString();
	}
}
