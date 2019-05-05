using UnityEngine;
using UnityEngine.UI;

public class PopulateRobotsGrid : MonoBehaviour {
	public Image robotImage;
	public Text robotText;

	public static string[] robots = { "Kiddo", "Nourinha", "Dozzer", "Breach" };
	Player player;

	void Start() {
		for (int i = 0; i < robots.Length / 2; i++) {

		}
		for (int i = 0; i < robots.Length / 2; i++) {

		}
	}

	public void LoadKiddo() {
		FindObjectOfType<Player>().robotModel = "Kiddo";
		robotText.text = TextManager.FormatText("<h1>Kiddo</h1>");
	}

	public void LoadDozzer() {
		FindObjectOfType<Player>().robotModel = "Dozzer";
		robotText.text = TextManager.FormatText("<h1>Dozzer</h1>");
	}
}
