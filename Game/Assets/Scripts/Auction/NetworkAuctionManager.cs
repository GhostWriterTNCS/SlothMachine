using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct Pair {
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
}

public class NetworkAuctionManager : NetworkBehaviour {
	public GameObject upgradeBoxPrefab;

	[Space]
	public int countdownDuration = 10;
	public string countdownText;
	[SyncVar]
	float currentCountdown;

	[Space]
	public int pauseDuration = 5;
	public string pauseText;
	[SyncVar]
	float currentPause;

	[Space]
	[SyncVar]
	public GameObject auctionWinner;
	[SyncVar]
	public int maxBid;

	int currentUpgrade = 0;
	List<UpgradeBox> upgrades = new List<UpgradeBox>();
	List<Pair> usedUpgrades = new List<Pair>();
	AuctionManager auctionManager;

	void Start() {
		auctionManager = FindObjectOfType<AuctionManager>();
		auctionManager.networkAuctionManager = this;
		auctionManager.scrapsInput.SetActive(true);
		auctionManager.scrapsWait.SetActive(false);
		auctionManager.scrapsList.SetActive(false);
	}

	[Command]
	public void CmdLoad() {
		for (int i = 0; i < 4; i++) {
			GameObject newPlayer = Instantiate(upgradeBoxPrefab);
			NetworkServer.Spawn(newPlayer);
			UpgradeBox ub = newPlayer.GetComponent<UpgradeBox>();
			int upgrade, level;
			if (i < 2) {
				level = 2;
			} else {
				level = 1;
			}
			do {
				upgrade = Random.Range(1, Upgrades.permanent[level].Length);
			} while (usedUpgrades.Contains(new Pair(level, upgrade)));
			usedUpgrades.Add(new Pair(level, upgrade));
			ub.ID = upgrade;
			ub.level = level;
			ub.selected = (i == 0);
			upgrades.Add(ub);
		}

		currentCountdown = countdownDuration;
		RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown).ToString()));

		currentPause = pauseDuration;
		StartCoroutine(AuctionCoroutine());
	}

	IEnumerator AuctionWinnerCoroutine() {
		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			while (!pb.bidRegistered && !pb.player.upgradeAssigned) {
				yield return new WaitForSeconds(0.05f);
			}
			if (pb.bid > pb.player.scraps) {
				pb.bid = pb.player.scraps;
			}
			if (pb.bid > maxBid) {
				maxBid = pb.bid;
				auctionWinner = pb.gameObject;
			}
		}
		if (auctionWinner) {
			AuctionPlayer playerBox = auctionWinner.GetComponent<AuctionPlayer>();
			playerBox.player.scraps -= playerBox.bid;
		}
	}

	[ClientRpc]
	public void RpcSetHeader(string s) {
		auctionManager.header.text = s;
	}

	[ClientRpc]
	public void RpcUpgradeBoxSetParent(GameObject go) {
		go.transform.SetParent(auctionManager.upgradesList.transform);
	}

	[ClientRpc]
	public void RpcCountdownFinished() {
		if (FindObjectOfType<ScrapsInput>()) {
			FindObjectOfType<ScrapsInput>().SendBidValue();
		}
		auctionManager.scrapsInput.SetActive(false);
		auctionManager.scrapsWait.SetActive(true);
		auctionManager.CalculateAgentsBids();
	}

	[ClientRpc]
	public void RpcUpdateResults() {
		auctionManager.UpdateResults();
		auctionManager.scrapsWait.SetActive(false);
		auctionManager.scrapsList.SetActive(true);
	}

	[ClientRpc]
	public void RpcPauseFinished() {
		Player player = null;
		foreach (AuctionPlayer ap in FindObjectsOfType<AuctionPlayer>()) {
			if (ap.GetComponent<NetworkIdentity>().isLocalPlayer) {
				player = ap.player;
				break;
			}
		}
		auctionManager.scrapsInput.SetActive(!player.upgradeAssigned);
		auctionManager.scrapsWait.SetActive(player.upgradeAssigned);
		auctionManager.scrapsList.SetActive(false);
		ScrapsInput si = FindObjectOfType<ScrapsInput>();
		if (si) {
			si.ResetValue();
		}
		foreach (UpgradeBox ub in FindObjectsOfType<UpgradeBox>()) {
			ub.RefreshSelected();
		}
	}

	IEnumerator AuctionCoroutine() {
		while (currentCountdown > 0) {
			currentCountdown -= Time.deltaTime;
			RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown + 1).ToString()));
			yield return new WaitForEndOfFrame();
		}
		RpcCountdownFinished();
		StartCoroutine(AuctionWinnerCoroutine());
		RpcUpdateResults();
		while (auctionWinner == null) {
			yield return new WaitForSeconds(0.05f);
		}
		auctionWinner.GetComponent<AuctionPlayer>().player.CmdAddUpgrade(usedUpgrades[currentUpgrade].value1, usedUpgrades[currentUpgrade].value2);
		RpcSetHeader(pauseText.Replace("#", auctionWinner.GetComponent<AuctionPlayer>().player.name));
		while (currentPause > 0) {
			currentPause -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			pb.bid = 0;
			pb.bidRegistered = false;
		}
		currentUpgrade++;
		maxBid = 0;
		auctionWinner = null;
		for (int i = 0; i < upgrades.Count; i++) {
			upgrades[i].selected = (i == currentUpgrade);
			upgrades[i].isUpdated = true;
		}
		RpcPauseFinished();
		currentCountdown = countdownDuration;
		currentPause = pauseDuration;
		if (currentUpgrade < 3) {
			StartCoroutine(AuctionCoroutine());
		} else {
			foreach (Player p in FindObjectsOfType<Player>()) {
				if (!p.upgradeAssigned) {
					p.CmdAddUpgrade(upgrades[3].level, upgrades[3].ID);
					break;
				}
			}
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		}
	}
}
