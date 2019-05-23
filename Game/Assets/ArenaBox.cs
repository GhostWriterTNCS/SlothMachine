using UnityEngine;
using UnityEngine.UI;

public class ArenaBox : MonoBehaviour {
	/*[SyncVar]
	public GameObject playerGO;*/
	public Player player;

	public Image robotImage;
	public Text nameText;
	public Text scoreText;
	public Slider scoreSlider;

	void Start() {
		robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
		nameText.text = player.name;
	}

	void Update() {
		scoreText.text = player.score.ToString();
		scoreSlider.value = player.score;
	}
}
