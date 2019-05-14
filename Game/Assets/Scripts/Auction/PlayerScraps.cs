﻿using System.Collections;
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

	/*[SyncVar]
	public GameObject highestBid;*/

	void Start() {
		StartCoroutine(LoadPlayer());
	}

	IEnumerator LoadPlayer() {
		while (!playerBoxGO) {
			yield return new WaitForSeconds(0.05f);
		}
		transform.SetParent(FindObjectOfType<AuctionManager>().scrapsList.transform);
		playerBox = playerBoxGO.GetComponent<AuctionPlayer>();
		playerName.text = playerBox.player.name;
	}

	public void UpdateResult() {
		StartCoroutine(UpdateResultCoroutine());
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
		}
		scrapsSlider.value = playerBox.bid / (float)NAM.maxBid;
		scrapsValue.text = playerBox.bid.ToString();
	}
}
