using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pair {
	public int value1;
	public int value2;

	public Pair(int v1, int v2) {
		value1 = v1;
		value2 = v2;
	}

	public override bool Equals(object obj) {
		if (obj == null)
			return false;
		if (GetType() != obj.GetType()) return false;

		Pair p = (Pair)obj;
		return (value1 == p.value1) && (value2 == p.value2);
	}

	public static implicit operator bool(Pair obj) {
		return obj != null;
	}
}

public class NetworkAuctionManager : NetworkBehaviour {
	public GameObject upgradeBoxPrefab;
	public GameObject upgradeBoxWithDescPrefab;
	public int introDuration = 5;
	public string introText;
	[SyncVar]
	float currentIntro;

	[Space]
	public int countdownDuration = 7;
	public string countdownText;
	[SyncVar]
	float currentCountdown;

	[Space]
	public int pauseDuration = 5;
	public string pauseText;
	public string upgradeLost;

	[Space]
	[SyncVar]
	public short maxBid;
	[SyncVar]
	public GameObject auctionWinner;
	[SyncVar]
	public bool auctionRegistered;
	[SyncVar]
	public string currentTitle;

	[SyncVar]
	float currentPause;
	[SyncVar]
	byte currentUpgrade = 0;
	bool isUpgradeLost;
	List<Pair> usedUpgradesTemp = new List<Pair>();
	public List<UpgradeBox> upgrades = new List<UpgradeBox>();
	public SyncListInt auctionUpgrades = new SyncListInt();

	AuctionManager auctionManager;

	void Start() {
		auctionManager = FindObjectOfType<AuctionManager>();
		auctionManager.networkAuctionManager = this;
		CmdLoad();
	}

	[Command]
	void CmdLoad() {
		//Debug.Log("Player count: " + MatchManager.singleton.playerCount);
		for (int i = 0; i < MatchManager.singleton.playerCount; i++) {
			int upgrade, level;
			if (i < MatchManager.singleton.playerCount / 2) {
				level = MatchManager.singleton.roundCounter < 2 ? 2 : 3;
			} else {
				level = MatchManager.singleton.roundCounter < 2 ? 1 : 2;
			}
			do {
				upgrade = Random.Range(1, Upgrades.permanent[level].Length);
			} while (usedUpgradesTemp.Contains(new Pair(level, upgrade)));
			usedUpgradesTemp.Add(new Pair(level, upgrade));
			auctionUpgrades.Add(level);
			auctionUpgrades.Add(upgrade);
		}
		//RpcCreateUpgradeBoxes();

		//RpcSetHeader(introText);
		currentTitle = introText;
		currentIntro = introDuration;
		currentCountdown = countdownDuration;
		currentPause = pauseDuration;
		StartCoroutine(AuctionCoroutine());
	}

	[ClientRpc]
	void RpcShowAuctionPanel() {
		FindObjectOfType<AuctionManager>().introPanel.SetActive(false);
		FindObjectOfType<AuctionManager>().auctionPanel.SetActive(true);
	}

	[ClientRpc]
	void RpcUpgradeBoxSetParent(GameObject go) {
		go.transform.SetParent(FindObjectOfType<AuctionManager>().upgradesList.transform);
	}

	[ClientRpc]
	void RpcCountdownFinished() {
		FindObjectOfType<AuctionManager>().scrapsInput.GetComponent<ScrapsInput>().SendBidValue();
		FindObjectOfType<AuctionManager>().scrapsInput.SetActive(false);
		FindObjectOfType<AuctionManager>().scrapsWait.SetActive(true);
		FindObjectOfType<AuctionManager>().CalculateAgentsBids();
	}

	[ClientRpc]
	void RpcUpdateResults() {
		FindObjectOfType<AuctionManager>().UpdateResults();
		FindObjectOfType<AuctionManager>().scrapsWait.SetActive(false);
		FindObjectOfType<AuctionManager>().scrapsList.SetActive(true);
	}

	[ClientRpc]
	void RpcPauseFinished() {
		Player player = null;
		foreach (AuctionPlayer ap in FindObjectsOfType<AuctionPlayer>()) {
			if (ap.GetComponent<NetworkIdentity>().isLocalPlayer) {
				player = ap.player;
				break;
			}
		}
		for (int i = 0; i < upgrades.Count; i++) {
			upgrades[i].selected = (i == currentUpgrade);
			upgrades[i].isUpdated = true;
		}
		FindObjectOfType<AuctionManager>().scrapsInput.SetActive(!player.upgradeAssigned);
		FindObjectOfType<AuctionManager>().scrapsWait.SetActive(player.upgradeAssigned);
		FindObjectOfType<AuctionManager>().scrapsList.SetActive(false);
		/*ScrapsInput si = FindObjectOfType<ScrapsInput>();
		if (si) {
			si.ResetValue();
		}*/
		Debug.Log("Upgrades: " + FindObjectsOfType<UpgradeBox>().Length);
		foreach (UpgradeBox ub in FindObjectsOfType<UpgradeBox>()) {
			ub.RefreshSelected();
		}
	}

	IEnumerator AuctionCoroutine() {
		maxBid = 0;
		auctionWinner = null;
		auctionRegistered = false;
		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			pb.bid = 0;
			pb.bidRegistered = false;
		}

		while (currentIntro > 0) {
			currentIntro -= Time.deltaTime;
			yield return 0;
		}
		RpcShowAuctionPanel();
		//RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown).ToString()));
		currentTitle = countdownText.Replace("#", ((int)currentCountdown).ToString());
		while (currentCountdown > 0) {
			currentCountdown -= Time.deltaTime;
			//RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown + 1).ToString()));
			currentTitle = countdownText.Replace("#", ((int)currentCountdown + 1).ToString());
			yield return 0;
		}
		RpcCountdownFinished();

		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			while (!pb.bidRegistered && !pb.player.upgradeAssigned) {
				Debug.Log("Waiting for players to send values");
				yield return 0;
			}
			if (pb.player.upgradeAssigned) {
				pb.bid = 0;
			}
			if (pb.bid > pb.player.scraps) {
				pb.bid = pb.player.scraps;
			}
			if (pb.bid > maxBid) {
				maxBid = pb.bid;
				auctionWinner = pb.gameObject;
			}
		}
		auctionRegistered = true;
		RpcUpdateResults();
		if (auctionWinner) {
			AuctionPlayer playerBox = auctionWinner.GetComponent<AuctionPlayer>();
			playerBox.player.scraps -= playerBox.bid;
			auctionWinner.GetComponent<AuctionPlayer>().player.CmdAddPermanentUpgrade(auctionUpgrades[currentUpgrade * 2], auctionUpgrades[currentUpgrade * 2 + 1]);
			//RpcSetHeader(pauseText.Replace("#", auctionWinner.GetComponent<AuctionPlayer>().player.name));
			currentTitle = pauseText.Replace("#", auctionWinner.GetComponent<AuctionPlayer>().player.name);
		} else {
			isUpgradeLost = true;
			//RpcSetHeader(upgradeLost);
			currentTitle = upgradeLost;
		}

		/*foreach (Player p in FindObjectsOfType<Player>()) {
			Player other = auctionManager.GetComponent<AuctionPlayer>().player;
			if (!p.expectedMoney.ContainsKey(other)) {
				p.expectedMoney.Add(other, Player.defaultScraps);
			}
			p.expectedMoney[other] -= maxBid;
		}*/

		currentUpgrade++;
		while (currentPause > 0) {
			currentPause -= Time.deltaTime;
			yield return 0;
		}
		RpcPauseFinished();

		countdownDuration--;
		currentCountdown = countdownDuration;
		currentPause = pauseDuration;
		int size = upgrades.Count - 1;
		if (currentUpgrade < size || currentUpgrade < upgrades.Count && isUpgradeLost) {
			StartCoroutine(AuctionCoroutine());
		} else {
			foreach (Player p in FindObjectsOfType<Player>()) {
				if (!p.upgradeAssigned) {
					p.CmdAddPermanentUpgrade(upgrades[size].level, upgrades[size].ID);
					break;
				}
			}
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		}
	}
}
