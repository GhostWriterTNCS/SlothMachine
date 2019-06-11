﻿using System.Collections;
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
	public GameObject upgradeBoxWithDescPrefab;
	public int introDuration = 5;
	public string introText;
	[SyncVar]
	float currentIntro;

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
		if (FindObjectsOfType<Player>().Length == 1) {
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		}
		auctionManager = FindObjectOfType<AuctionManager>();
		auctionManager.networkAuctionManager = this;
		auctionManager.scrapsInput.SetActive(true);
		auctionManager.scrapsWait.SetActive(false);
		auctionManager.scrapsList.SetActive(false);
		auctionManager.introPanel.SetActive(true);
		auctionManager.auctionPanel.SetActive(false);
	}

	[Command]
	public void CmdLoad() {
		for (int i = 0; i < FindObjectsOfType<Player>().Length; i++) {
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
			GameObject presUpgrade = Instantiate(upgradeBoxWithDescPrefab);
			NetworkServer.Spawn(presUpgrade);
			ub = presUpgrade.GetComponent<UpgradeBox>();
			ub.ID = upgrade;
			ub.level = level;
			ub.isIntro = true;
		}

		RpcSetHeader(introText);
		currentIntro = introDuration;
		currentCountdown = countdownDuration;
		//RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown).ToString()));
		currentPause = pauseDuration;
		StartCoroutine(AuctionCoroutine());
	}

	IEnumerator AuctionWinnerCoroutine() {
		foreach (AuctionPlayer pb in FindObjectsOfType<AuctionPlayer>()) {
			while (!pb.bidRegistered && !pb.player.upgradeAssigned) {
				yield return 0;
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
		FindObjectOfType<AuctionManager>().header.text = s;
	}

	[ClientRpc]
	public void RpcShowAuctionPanel() {
		FindObjectOfType<AuctionManager>().introPanel.SetActive(false);
		FindObjectOfType<AuctionManager>().auctionPanel.SetActive(true);
	}

	[ClientRpc]
	public void RpcUpgradeBoxSetParent(GameObject go) {
		go.transform.SetParent(FindObjectOfType<AuctionManager>().upgradesList.transform);
	}

	[ClientRpc]
	public void RpcCountdownFinished() {
		if (FindObjectOfType<ScrapsInput>()) {
			FindObjectOfType<ScrapsInput>().SendBidValue();
		}
		FindObjectOfType<AuctionManager>().scrapsInput.SetActive(false);
		FindObjectOfType<AuctionManager>().scrapsWait.SetActive(true);
		FindObjectOfType<AuctionManager>().CalculateAgentsBids();
	}

	[ClientRpc]
	public void RpcUpdateResults() {
		FindObjectOfType<AuctionManager>().UpdateResults();
		FindObjectOfType<AuctionManager>().scrapsWait.SetActive(false);
		FindObjectOfType<AuctionManager>().scrapsList.SetActive(true);
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
		FindObjectOfType<AuctionManager>().scrapsInput.SetActive(!player.upgradeAssigned);
		FindObjectOfType<AuctionManager>().scrapsWait.SetActive(player.upgradeAssigned);
		FindObjectOfType<AuctionManager>().scrapsList.SetActive(false);
		ScrapsInput si = FindObjectOfType<ScrapsInput>();
		if (si) {
			si.ResetValue();
		}
		foreach (UpgradeBox ub in FindObjectsOfType<UpgradeBox>()) {
			ub.RefreshSelected();
		}
	}

	IEnumerator AuctionCoroutine() {
		while (currentIntro > 0) {
			currentIntro -= Time.deltaTime;
			yield return 0;
		}
		RpcShowAuctionPanel();
		RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown).ToString()));
		while (currentCountdown > 0) {
			currentCountdown -= Time.deltaTime;
			RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown + 1).ToString()));
			yield return 0;
		}
		RpcCountdownFinished();
		StartCoroutine(AuctionWinnerCoroutine());
		RpcUpdateResults();
		while (auctionWinner == null) {
			yield return 0;
		}
		auctionWinner.GetComponent<AuctionPlayer>().player.CmdAddPermanentUpgrade(usedUpgrades[currentUpgrade].value1, usedUpgrades[currentUpgrade].value2);
		RpcSetHeader(pauseText.Replace("#", auctionWinner.GetComponent<AuctionPlayer>().player.name));
		while (currentPause > 0) {
			currentPause -= Time.deltaTime;
			yield return 0;
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
		int size = upgrades.Count - 1;
		if (currentUpgrade < size) {
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
