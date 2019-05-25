using System.Collections;
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
	public Image sliderImage;

	void Start() {
		StartCoroutine(FixHeight());
		robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
		nameText.text = player.name;
	}

	IEnumerator FixHeight() {
		while (robotImage.GetComponent<RectTransform>().rect == Rect.zero) {
			yield return new WaitForEndOfFrame();
		}
		robotImage.GetComponent<LayoutElement>().minWidth = robotImage.GetComponent<RectTransform>().rect.height;
	}

	void Update() {
		scoreText.text = player.score.ToString();
		scoreSlider.value = player.score;
		//sliderImage.preferredWidth = player.score;
	}
}
