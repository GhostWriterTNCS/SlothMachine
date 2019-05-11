using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrapsInput : MonoBehaviour {
	public Text input;
	public Player player;

	int value;

	void Start() {
		value = Convert.ToInt32(input.text);
	}

	void UpdateText() {
		player.name = value.ToString();
		input.text = value + "/" + player.scraps;
	}

	public void SetPlayer(Player player) {
		this.player = player;
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
}
