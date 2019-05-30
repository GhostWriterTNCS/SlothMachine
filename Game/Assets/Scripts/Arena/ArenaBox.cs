using UnityEngine;
using UnityEngine.UI;

public class ArenaBox : MonoBehaviour {
	public Player player;
	public Robot robot;

	public Image robotImage;
	public Text nameText;
	public Text scoreText;
	public Slider scoreSlider;
	//public Image sliderImage;

	void Start() {
		//StartCoroutine(FixHeight());
		robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
		nameText.text = player.name;
		robot = player.GetComponentInChildren<Robot>();
	}

	/*IEnumerator FixHeight() {
		while (robotImage.GetComponent<RectTransform>().rect == Rect.zero) {
			yield return new WaitForEndOfFrame();
		}
		robotImage.GetComponent<LayoutElement>().minWidth = robotImage.GetComponent<RectTransform>().rect.height;
	}*/

	void Update() {
		// Show only the score for the current round.
		scoreText.text = robot.roundScore.ToString();
		scoreSlider.value = robot.roundScore;
	}
}
