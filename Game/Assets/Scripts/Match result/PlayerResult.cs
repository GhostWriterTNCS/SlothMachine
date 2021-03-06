﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerResult : NetworkBehaviour {
	public Slider scoreSlider;
	public Text nameText;
	public Image robotImage;
	public Text statsText;
	public Text finalScoreText;

	[SyncVar]
	public GameObject playerGO;
	public Player player;

	int finalScore;
	static int maxFinalScore = 0;

	void Start() {
		transform.SetParent(FindObjectOfType<MatchResultManager>().container);
		StartCoroutine(LoadPlayer());
	}

	public IEnumerator LoadPlayer() {
		while (FindObjectOfType<Canvas>().transform.localScale.x < 0.1f) {
			yield return 0;
		}
		transform.localScale = Vector3.one; // FindObjectOfType<Canvas>().transform.localScale;
		while (!playerGO) {
			yield return 0;
		}
		player = playerGO.GetComponent<Player>();
		nameText.text = player.name;
		robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
		string stats = statsText.text;
		stats = stats.Replace("#1", player.score.ToString());
		stats = stats.Replace("#2", player.roundWinner.ToString());
		stats = stats.Replace("#3", player.scraps.ToString());
		stats = stats.Replace("#4", player.deathCount.ToString());
		statsText.text = stats;
		finalScore = player.score + player.roundWinner * 250 + player.scraps * 2 - player.deathCount * 50;
		if (finalScore < 0) {
			finalScore = 0;
		}
		finalScoreText.text = finalScoreText.text.Replace("#", finalScore.ToString());
		if (finalScore > maxFinalScore) {
			maxFinalScore = finalScore;
		}
		FindObjectOfType<MatchResultManager>().backToStart.gameObject.SetActive(true);
	}

	void Update() {
		scoreSlider.value = finalScore / (float)maxFinalScore;
	}
}
