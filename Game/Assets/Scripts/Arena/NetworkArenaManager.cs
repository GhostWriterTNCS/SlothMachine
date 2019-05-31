using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkArenaManager : NetworkBehaviour {
	ArenaManager arenaManager;
	[SyncVar]
	public float roundDuration;

	void Start() {
		CmdStartCoroutine();
	}

	[Command]
	public void CmdStartCoroutine() {
		StartCoroutine(Run());
	}

	IEnumerator Run() {
		arenaManager = FindObjectOfType<ArenaManager>();
		GameObject arena;
		if (FindObjectOfType<MatchManager>().roundCounter > 0) {
			arena = Instantiate(arenaManager.arenaPrefabs[UnityEngine.Random.Range(0, arenaManager.arenaPrefabs.Length)]);
		} else {
			arena = Instantiate(arenaManager.bossArena);
		}
		NetworkServer.Spawn(arena);
		roundDuration = arenaManager.roundDuration;
		string title = arenaManager.finalRound;
		if (MatchManager.singleton.roundCounter > 0) {
			title = arenaManager.roundX.Replace("#", MatchManager.singleton.roundCounter.ToString());
		}
		arenaManager.title.gameObject.SetActive(true);
		arenaManager.title.text = title;
		RpcUpdateTitle(title);
		yield return new WaitForSeconds(2);
		RpcTitleSetActive(false);
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			r.paused = false;
		}
		while (roundDuration >= 0) {
			roundDuration -= Time.fixedDeltaTime;
			TimeSpan time = TimeSpan.FromSeconds(roundDuration + 1);
			RpcUpdateCountdown(time.ToString(@"mm\:ss"));
			yield return new WaitForFixedUpdate();
		}
		int scoreMax = -1;
		Player roundWinner = null;
		foreach (Player p in FindObjectsOfType<Player>()) {
			p.GetComponentInChildren<Robot>().paused = true;
			if (p.score > scoreMax) {
				scoreMax = p.score;
				roundWinner = p;
			}
		}
		roundWinner.roundWinner++;
		RpcUpdateCountdown("");
		RpcUpdateTitle(arenaManager.roundWinnerIs.Replace("\\n", "\n").Replace("#", roundWinner.name));
		yield return new WaitForSeconds(5);
		string scene = GameScenes.Auction;
		if (roundWinner.roundWinner >= 2) {
			MatchManager.singleton.roundCounter = -1;
			scene = GameScenes.Arena;
		}
		NetworkManager.singleton.ServerChangeScene(scene);
	}

	[ClientRpc]
	public void RpcUpdateTitle(string s) {
		FindObjectOfType<ArenaManager>().title.gameObject.SetActive(true);
		FindObjectOfType<ArenaManager>().title.text = s;
	}
	[ClientRpc]
	public void RpcTitleSetActive(bool value) {
		FindObjectOfType<ArenaManager>().title.gameObject.SetActive(value);
	}
	[ClientRpc]
	public void RpcUpdateCountdown(string s) {
		FindObjectOfType<ArenaManager>().countdown.text = s;
	}

	[Command]
	public void CmdSpawnWorm(GameObject prefab, Vector3 pos) {
		Debug.Log("Spawn worm");
		transform.position = new Vector3(150, FindObjectOfType<Terrain>().terrainData.GetHeight(150, 150) + 1, 150);
		GameObject worm = Instantiate(prefab, pos, Quaternion.identity);
		NetworkServer.Spawn(worm);
		worm.GetComponent<Rigidbody>().AddForce(0, 2500, 0);
	}
}
