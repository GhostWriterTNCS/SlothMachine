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
	[SyncVar]
	public GameObject auctionWinner;
	[SyncVar]
	public int maxBid;

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

	int currentUpgrade = 0;
	List<UpgradeBox> upgrades = new List<UpgradeBox>();
	List<Pair> usedUpgrades = new List<Pair>();

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
				upgrade = Random.Range(1, Upgrades.list[level].Length);
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
		foreach (PlayerBox pb in FindObjectsOfType<PlayerBox>()) {
			while (!pb.bidRegistered) {
				yield return new WaitForSeconds(0.05f);
			}
			if (pb.bid > maxBid) {
				maxBid = pb.bid;
				auctionWinner = pb.gameObject;
			}
		}
	}

	[ClientRpc]
	public void RpcSetHeader(string s) {
		FindObjectOfType<AuctionManager>().header.text = s;
	}

	[ClientRpc]
	public void RpcUpgradeBoxSetParent(GameObject go) {
		go.transform.SetParent(FindObjectOfType<AuctionManager>().upgradesList.transform);
	}

	[ClientRpc]
	public void RpcCountdownFinished() {
		FindObjectOfType<ScrapsInput>().SendBidValue();
		FindObjectOfType<SwitchGameObjects>().Switch();
		FindObjectOfType<AuctionManager>().CalculateAgentsBids();
	}

	[ClientRpc]
	public void RpcUpdateResults() {
		FindObjectOfType<AuctionManager>().UpdateResults();
	}

	[ClientRpc]
	public void RpcPauseFinished() {
		FindObjectOfType<SwitchGameObjects>().Switch();
		for (int i = 0; i < upgrades.Count; i++) {
			upgrades[i].selected = (i == currentUpgrade);
			upgrades[i].RefreshSelected();
		}
	}

	IEnumerator AuctionCoroutine() {
		while (currentCountdown > 0) {
			currentCountdown -= Time.deltaTime;
			RpcSetHeader(countdownText.Replace("#", ((int)currentCountdown).ToString()));
			yield return new WaitForEndOfFrame();
		}
		RpcCountdownFinished();
		StartCoroutine(AuctionWinnerCoroutine());
		RpcUpdateResults();
		while (auctionWinner == null) {
			yield return new WaitForSeconds(0.05f);
		}
		RpcSetHeader(pauseText.Replace("#", auctionWinner.GetComponent<PlayerBox>().player.name));
		while (currentPause > 0) {
			currentPause -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		foreach (PlayerBox pb in FindObjectsOfType<PlayerBox>()) {
			pb.bidRegistered = false;
		}
		auctionWinner = null;
		currentUpgrade++;
		RpcPauseFinished();
		currentCountdown = countdownDuration;
		currentPause = pauseDuration;
		if (currentUpgrade < 3) {
			StartCoroutine(AuctionCoroutine());
		} else {
			NetworkManager.singleton.ServerChangeScene(GameScenes.Arena);
		}
	}
}
