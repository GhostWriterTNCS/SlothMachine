﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerBox : NetworkBehaviour {
	[SyncVar]
	public GameObject playerGO;
	public Player player;

	public Image robotImage;
	public Text nameText;
	public Text scoreText;
	public Slider scoreSlider;

	//public GameObject scrapsBlock;

	[SyncVar]
	public bool bidRegistered = false;
	[SyncVar]
	public int bid;

	void Start() {
		StartCoroutine(LoadPlayer());
	}

	IEnumerator LoadPlayer() {
		while (!playerGO) {
			yield return new WaitForSeconds(0.01f);
		}
		transform.SetParent(FindObjectOfType<AuctionManager>().playersList.transform);
		Debug.Log("Load player " + playerGO.name);
		player = playerGO.GetComponent<Player>();
		if (player) {
			robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
			nameText.text = player.name;
			scoreText.text = player.score.ToString();
			float maxScore = 0;
			foreach (Player p in FindObjectsOfType<Player>()) {
				if (p.score > maxScore) {
					maxScore = p.score;
				}
			}
			scoreSlider.value = player.score / maxScore;
			if (isLocalPlayer) {
				FindObjectOfType<ScrapsInput>().SetPlayerBox(this);
			}
		}
	}

	[Command]
	public void CmdSetBid(int value) {
		Debug.Log("Set bid: " + value);
		bid = value;
		bidRegistered = true;
	}
}