using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResult : MonoBehaviour {
	public Slider scoreSlider;
	public Text nameText;
	public Image robotImage;
	public Text statsText;
	public Text finalScoreText;

	public GameObject playerGO;
	public Player player;

	int finalScore;
	static int maxFinalScore = 0;

	void Start() {
		transform.SetParent(FindObjectOfType<MatchResultManager>().container);
		StartCoroutine(LoadPlayer());
	}

	public IEnumerator LoadPlayer() {
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
		finalScoreText.text = finalScore.ToString();
		if (finalScore > maxFinalScore) {
			maxFinalScore = finalScore;
		}
		FindObjectOfType<MatchResultManager>().backToStart.gameObject.SetActive(true);
	}

	void Update() {
		scoreSlider.value = finalScore / (float)maxFinalScore;
	}
}
