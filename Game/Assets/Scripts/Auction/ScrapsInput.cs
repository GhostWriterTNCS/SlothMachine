﻿using UnityEngine.Networking;
using UnityEngine.UI;

public class ScrapsInput : NetworkBehaviour {
	public Text input;
	public Player player;
	public PlayerBox playerBox;

	[SyncVar]
	public bool upgradeAssigned;

	int value;

	void Start() {
		upgradeAssigned = false;
	}

	void UpdateText() {
		input.text = value + "/" + player.scraps;
	}

	public void SetPlayerBox(PlayerBox playerBox) {
		this.playerBox = playerBox;
		player = playerBox.player;
		UpdateText();
	}

	public void Increase() {
		if (value < player.scraps) {
			value++;
			UpdateText();
		}
	}
	public void Decrease() {
		if (value > 0) {
			value--;
			UpdateText();
		}
	}

	public void ResetValue() {
		value = 0;
		UpdateText();
	}

	public void SendBidValue() {
		playerBox.CmdSetBid(value);
	}
}
