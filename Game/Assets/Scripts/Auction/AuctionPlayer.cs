using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuctionPlayer : NetworkBehaviour {
	[SyncVar]
	public GameObject playerGO;
	public Player player;

	public Image robotImage;
	public Text nameText;
	public Text scoreText;
	public Slider scoreSlider;
	public Image[] upgrades;

	[SyncVar]
	public bool bidRegistered = false;
	[SyncVar]
	public int bid;

	void Start() {
		StartCoroutine(LoadPlayer());
	}

	IEnumerator LoadPlayer() {
		while (!playerGO) {
			yield return new WaitForSeconds(0.05f);
		}
		transform.SetParent(FindObjectOfType<AuctionManager>().playersList.transform);
		Debug.Log("Load player " + playerGO.name);
		player = playerGO.GetComponent<Player>();
		if (player) {
			robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
			nameText.text = player.name;
			scoreText.text = player.score.ToString();
			float maxScore = 0;
			foreach (Player p in FindObjectsOfType<Player>()) {
				if (p.score > maxScore) {
					maxScore = p.score;
				}
			}
			scoreSlider.value = player.score / maxScore;
			if (isLocalPlayer) {
				while (!FindObjectOfType<ScrapsInput>()) {
					yield return new WaitForSeconds(0.05f);
				}
				FindObjectOfType<ScrapsInput>().SetPlayerBox(this);
			}
		}
		foreach (Image i in upgrades) {
			i.enabled = false;
		}
	}

	[Command]
	public void CmdSetBid(int value) {
		bid = value;
		bidRegistered = true;
	}

	public void ShowUpgrade(int index) {
		StartCoroutine(ShowUpgradeCoroutine(index));
	}
	IEnumerator ShowUpgradeCoroutine(int index) {
		while (player.upgrades.Count <= index) {
			yield return new WaitForSeconds(0.05f);
		}
		upgrades[index].sprite = Resources.Load<Sprite>("UI/Upgrades/" + player.upgrades[index].value1 + "_" + player.upgrades[index].value2);
		upgrades[index].enabled = true;
	}
}
