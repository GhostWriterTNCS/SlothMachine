﻿using UnityEngine;
using UnityEngine.UI;

public class ArenaBox : MonoBehaviour {
	public Player player;
	public Robot robot;

	public Image robotImage;
	public Image robotImageFrame;
	public Text nameText;
	public Image roundWinnerImage1;
	public Image roundWinnerImage2;
	public Text scoreText;
	public Slider scoreSlider;
	public Image backgroundImage;

	void Start() {
		backgroundImage.gameObject.SetActive(false);
		robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
		robotImageFrame.color = player.color;
		nameText.text = player.name;
		if (player.roundWinner <= 1) {
			roundWinnerImage2.gameObject.SetActive(false);
			if (player.roundWinner == 0) {
				roundWinnerImage1.gameObject.SetActive(false);
			}
		}
		robot = player.GetComponentInChildren<Robot>();
		if (robot.isLocalPlayer && !robot.player.isAgent) {
			backgroundImage.gameObject.SetActive(true);
			backgroundImage.color = TextManager.backgroundHighlightedColor;
		}
	}

	void Update() {
		// Show only the score for the current round.
		scoreText.text = robot.roundScore.ToString();
		scoreSlider.value = robot.roundScore;
	}
}
