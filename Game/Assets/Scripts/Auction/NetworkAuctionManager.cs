using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/*public struct Pair {
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
}*/

public class NetworkAuctionManager : NetworkBehaviour {
	public GameObject upgradeBoxPrefab;
	public Text header;

	[Space]
	public int countdownDuration = 10;
	public string countdownText;
	float currentCountdown;

	[Space]
	public int pauseDuration = 5;
	public string pauseText;
	float currentPause;

	List<Pair> usedUpgrades = new List<Pair>();

	void Start() {
		StartCoroutine(LoadCoroutine());
	}

	IEnumerator LoadCoroutine() {
		yield return new WaitForSeconds(0.5f);

		Debug.Log(FindObjectsOfType<Player>().Length);

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
			ub.LoadUpgrade();
			if (i == 0) {
				ub.backgroundImage.enabled = true;
			} else {
				ub.backgroundImage.enabled = false;
			}
		}

		currentCountdown = countdownDuration;
		header.text = countdownText.Replace("#", ((int)currentCountdown).ToString());

		currentPause = pauseDuration;
		StartCoroutine(AuctionCoroutine());
	}

	GameObject auctionWinner;
	[ClientRpc]
	public void RpcCountdownFinished() {
		FindObjectOfType<ScrapsInput>().SendBidValue();
		//auctionWinner = FindObjectOfType<PlayerScraps>().gameObject;
		FindObjectOfType<SwitchGameObjects>().Switch();
		FindObjectOfType<AuctionManager>().EvaluateBids();
	}

	[ClientRpc]
	public void RpcPauseFinished() {
		FindObjectOfType<SwitchGameObjects>().Switch();
	}

	IEnumerator AuctionCoroutine() {
		while (currentCountdown > 0) {
			currentCountdown -= Time.deltaTime;
			header.text = countdownText.Replace("#", ((int)currentCountdown).ToString());
			yield return new WaitForEndOfFrame();
		}
		RpcCountdownFinished();
		while (currentPause > 0) {
			currentPause -= Time.deltaTime;
			header.text = pauseText.Replace("#", "");
			yield return new WaitForEndOfFrame();
		}
		RpcPauseFinished();
		currentCountdown = countdownDuration;
		currentPause = pauseDuration;
		StartCoroutine(AuctionCoroutine());
	}
}
