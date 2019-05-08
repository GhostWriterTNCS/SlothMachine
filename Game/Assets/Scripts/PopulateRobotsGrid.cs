using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PopulateRobotsGrid : NetworkBehaviour {
	public Image robotImage;
	public Text[] playerTexts;

	//public static string[] robots = { "Kiddo", "Nourinha", "Dozzer", "Breach" };
	Player player;

	void Start() {
		/*for (int i = 0; i < robots.Length / 2; i++) {

		}
		for (int i = 0; i < robots.Length / 2; i++) {

		}*/
		StartCoroutine(LoadDefault());
	}

	int clientCount = 0;
	public override void OnStartClient() {
		Debug.LogError("Client started " + clientCount);
		clientCount++;
	}

	IEnumerator LoadDefault() {
		while (!FindObjectOfType<Player>()) {
			yield return new WaitForSeconds(0.01f);
		}
		LoadKiddo();
	}

	public void LoadKiddo() {
		Debug.Log(connectionToClient.connectionId + " - " + NetworkServer.connections.Count);
		FindObjectOfType<Player>().robotModel = "Kiddo";
		playerTexts[0].text = TextManager.FormatText("<h1>Kiddo</h1>");
	}

	public void LoadNourinha() {
		FindObjectOfType<Player>().robotModel = "Nourinha";
		playerTexts[0].text = TextManager.FormatText("<h1>Nourinha</h1>");
	}

	public void LoadBreach() {
		FindObjectOfType<Player>().robotModel = "Breach";
		playerTexts[0].text = TextManager.FormatText("<h1>Breach</h1>");
	}

	public void LoadDozzer() {
		FindObjectOfType<Player>().robotModel = "Dozzer";
		playerTexts[0].text = TextManager.FormatText("<h1>Dozzer</h1>");
	}
}
