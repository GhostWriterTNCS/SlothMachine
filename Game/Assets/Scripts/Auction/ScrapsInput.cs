using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScrapsInput : NetworkBehaviour {
	public Text input;
	public Player player;
	public PlayerBox playerBox;

	int value;

	void Start() {
		value = Convert.ToInt32(input.text);
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

	public void SendBidValue() {
		playerBox.CmdSetBid(value);
	}
}
