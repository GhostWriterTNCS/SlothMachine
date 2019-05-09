using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PopulateRobotsGrid : NetworkBehaviour {
	public Image robotImage;
	public Text[] playerTexts;

	public Prototype.NetworkLobby.LobbyPlayer lobbyPlayer;

	//public static string[] robots = { "Kiddo", "Nourinha", "Dozzer", "Breach" };
	Player player;

	void Start() {
		/*for (int i = 0; i < robots.Length / 2; i++) {

		}
		for (int i = 0; i < robots.Length / 2; i++) {

		}*/
		//StartCoroutine(LoadDefault());
	}

	int clientCount = 0;
	public override void OnStartClient() {
		Debug.LogError("Client started " + clientCount);
		clientCount++;
	}

	public IEnumerator LoadDefault() {
		while (lobbyPlayer == null) {
			yield return new WaitForSeconds(0.01f);
		}
		LoadKiddo();
	}

	public void LoadKiddo() {
		//Debug.Log(connectionToClient.connectionId + " - " + NetworkServer.connections.Count);
		lobbyPlayer.CmdNameChanged("Kiddo");
		/*FindObjectOfType<Player>().robotModel = "Kiddo";
		playerTexts[0].text = TextManager.FormatText("<h1>Kiddo</h1>");*/
	}

	public void LoadNourinha() {
		lobbyPlayer.CmdNameChanged("Nourinha");
		/*FindObjectOfType<Player>().robotModel = "Nourinha";
		playerTexts[0].text = TextManager.FormatText("<h1>Nourinha</h1>");*/
	}

	public void LoadBreach() {
		lobbyPlayer.CmdNameChanged("Breach");
		/*FindObjectOfType<Player>().robotModel = "Breach";
		playerTexts[0].text = TextManager.FormatText("<h1>Breach</h1>");*/
	}

	public void LoadDozzer() {
		lobbyPlayer.CmdNameChanged("Dozzer");
		/*FindObjectOfType<Player>().robotModel = "Dozzer";
		playerTexts[0].text = TextManager.FormatText("<h1>Dozzer</h1>");*/
	}
}
