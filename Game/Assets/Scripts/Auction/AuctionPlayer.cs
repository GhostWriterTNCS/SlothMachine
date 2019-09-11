using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct UpgradeInterest {
	public UpgradeBox upgradeBox;
	public float interest;

	public UpgradeInterest(UpgradeBox upgradeBox, float interest) {
		this.upgradeBox = upgradeBox;
		this.interest = interest;
	}


}

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
	public short bid;

	AuctionManager auctionManager;
	public AuctionAgentRobot auctionAgent;
	public List<UpgradeInterest> upgradeInterests = new List<UpgradeInterest>();

	void Start() {
		backgroundImage.gameObject.SetActive(false);
		auctionManager = FindObjectOfType<AuctionManager>();
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
		while (!FindObjectOfType<AuctionManager>()) {
			yield return 0;
		}
		AuctionManager auctionManager = FindObjectOfType<AuctionManager>();
		if (auctionManager.playersList.transform.childCount > 0) {
			Instantiate(Resources.Load<GameObject>("Prefabs/Separator horizontal"), auctionManager.playersList.transform);
		}
		transform.SetParent(auctionManager.playersList.transform);
		//Debug.Log("Load player " + playerGO.name);
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
			for (int i = 0; i < player.upgrades.Length; i++) {
				if (player.upgrades[i]) {
					ShowUpgrade(i);
				}
			}
			if (isLocalPlayer) {
				FindObjectOfType<AuctionManager>().scrapsInput.GetComponent<ScrapsInput>().SetPlayerBox(this);
				backgroundImage.gameObject.SetActive(true);
				backgroundImage.color = TextManager.backgroundHighlightedColor;
			}
		}
		auctionAgent = new AuctionAgentRobot(player);
		CmdEvaluateUpgrades();
	}

	[Command]
	public void CmdSetBid(int value) {
		bid = (short)value;
		bidRegistered = true;
	}

	public void ShowUpgrade(int index) {
		FindObjectOfType<AuctionManager>().StartCoroutine(ShowUpgradeCoroutine(index));
	}
	IEnumerator ShowUpgradeCoroutine(int index) {
		while (!player.upgrades[index]) {
			yield return 0;
		}
		upgrades[index].sprite = Resources.Load<Sprite>("UI/Upgrades/Permanent/" + player.upgrades[index].value1 + "_" + player.upgrades[index].value2);
		//upgrades[index].enabled = true;
		upgrades[index].color = Color.white;
	}

	[Command]
	public void CmdEvaluateUpgrades() {
		upgradeInterests.Clear();
		foreach (UpgradeBox u in FindObjectOfType<NetworkAuctionManager>().upgrades) {
			upgradeInterests.Add(new UpgradeInterest(u, auctionAgent.GetInterest(u, player, true)));
		}
		upgradeInterests = upgradeInterests.OrderByDescending(f => f.interest).ToList();
	}
}
