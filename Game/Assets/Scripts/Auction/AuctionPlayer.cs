using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuctionPlayer : NetworkBehaviour {
	[SyncVar]
	public GameObject playerGO;
	public Player player;

	public Image robotImage;
	public Image robotFrame;
	public Text nameText;
	public Image roundWinnerImage;
	public Text scoreText;
	public Slider scoreSlider;
	public Image[] upgrades;
	public Image backgroundImage;

	[SyncVar]
	public bool bidRegistered = false;
	[SyncVar]
	public int bid;

	void Start() {
		backgroundImage.gameObject.SetActive(false);
		StartCoroutine(LoadPlayer());
	}

	IEnumerator LoadPlayer() {
		while (!playerGO) {
			yield return 0;
		}
		while (FindObjectOfType<Canvas>().transform.localScale.x < 0.1f) {
			yield return 0;
		}
		transform.localScale = FindObjectOfType<Canvas>().transform.localScale;
		AuctionManager auctionManager = FindObjectOfType<AuctionManager>();
		if (auctionManager.playersList.transform.childCount > 0) {
			Instantiate(Resources.Load<GameObject>("Prefabs/Separator horizontal"), auctionManager.playersList.transform);
		}
		transform.SetParent(auctionManager.playersList.transform);
		Debug.Log("Load player " + playerGO.name);
		player = playerGO.GetComponent<Player>();
		if (player) {
			robotFrame.color = player.color;
			robotImage.sprite = Resources.Load<Sprite>("UI/Robots/" + player.robotName);
			nameText.text = player.name;
			if (player.roundWinner == 0) {
				roundWinnerImage.gameObject.SetActive(false);
			}
			scoreText.text = player.score.ToString();
			float maxScore = 0;
			foreach (Player p in FindObjectsOfType<Player>()) {
				if (p.score > maxScore) {
					maxScore = p.score;
				}
			}
			scoreSlider.value = player.score / maxScore;
			for (int i = 0; i < player.upgrades.Count; i++) {
				ShowUpgrade(i);
			}
			if (isLocalPlayer) {
				FindObjectOfType<AuctionManager>().scrapsInput.GetComponent<ScrapsInput>().SetPlayerBox(this);
				backgroundImage.gameObject.SetActive(true);
				backgroundImage.color = TextManager.backgroundHighlightedColor;
			}
		}
	}

	[Command]
	public void CmdSetBid(int value) {
		bid = value;
		bidRegistered = true;
	}

	public void ShowUpgrade(int index) {
		FindObjectOfType<AuctionManager>().StartCoroutine(ShowUpgradeCoroutine(index));
	}
	IEnumerator ShowUpgradeCoroutine(int index) {
		while (player.upgrades.Count <= index) {
			yield return 0;
		}
		upgrades[index].sprite = Resources.Load<Sprite>("UI/Upgrades/Permanent/" + player.upgrades[index].value1 + "_" + player.upgrades[index].value2);
		upgrades[index].enabled = true;
	}
}
