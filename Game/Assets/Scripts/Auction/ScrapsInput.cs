using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScrapsInput : NetworkBehaviour {
	public Text input;
	public Player player;
	public AuctionPlayer playerBox;

	[SyncVar]
	public bool upgradeAssigned;

	int value = 0;

	void Start() {
		upgradeAssigned = false;
	}

	float inputTimer = 0;
	private void Update() {
		if (!player) {
			return;
		}
		inputTimer -= Time.deltaTime;
		if (inputTimer <= 0) {
			if (Input.GetAxis("Vertical") >= 0.4f) {
				value += 10;
				inputTimer = ButtonTip.inputInterval;
			} else if (Input.GetAxis("Vertical") <= -0.4f) {
				value -= 10;
				inputTimer = ButtonTip.inputInterval;
			}
			if (value < 0) {
				value = 0;
			}
			if (value > player.scraps) {
				value = player.scraps;
			}
			UpdateText();
		}
	}

	void UpdateText() {
		input.text = value + "/" + player.scraps;
	}

	public void SetPlayerBox(AuctionPlayer playerBox) {
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
