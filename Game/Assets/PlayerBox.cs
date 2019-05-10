using UnityEngine;
using UnityEngine.UI;

public class PlayerBox : MonoBehaviour {
	[HideInInspector]
	public Player player;

	public Image robotImage;
	public Text nameText;
	public Text scoreText;
	public Slider scoreSlider;

	public GameObject scrapsBlock;

	void Start() {
		if (player) {
			robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
			nameText.text = player.playerID;
			scoreText.text = player.score.ToString();
			float maxScore = 0;
			foreach (Player p in FindObjectsOfType<Player>()) {
				if (p.score > maxScore) {
					maxScore = p.score;
				}
			}
			scoreSlider.value = player.score / maxScore;
		}
	}
}
