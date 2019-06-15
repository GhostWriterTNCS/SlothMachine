using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkArenaManager : NetworkBehaviour {
	ArenaManager arenaManager;
	[SyncVar]
	public float roundDuration;

	void Start() {
		CmdStartCoroutine();
	}

	[Command]
	public void CmdStartCoroutine() {
		UnityEngine.Random.InitState(System.Environment.TickCount);
		StartCoroutine(Run());
	}

	IEnumerator Run() {
		arenaManager = FindObjectOfType<ArenaManager>();
		GameObject arena;
		if (MatchManager.singleton.bossRound) {
			arena = Instantiate(arenaManager.bossArena);
		} else {
			arena = Instantiate(arenaManager.arenaPrefabs[UnityEngine.Random.Range(0, arenaManager.arenaPrefabs.Length)]);
		}
		NetworkServer.Spawn(arena);
		roundDuration = arenaManager.roundDuration;
		string title = arenaManager.finalRound;
		if (!MatchManager.singleton.bossRound) {
			title = arenaManager.roundX.Replace("#", MatchManager.singleton.roundCounter.ToString());
		}
		Debug.Log(title);
		//UpdateTitle(title);
		yield return new WaitForSeconds(1);
		RpcUpdateTitle(title);
		yield return new WaitForSeconds(2);
		if (MatchManager.singleton.bossRound) {
			//UpdateTitle(arenaManager.finalRoundOthers);
			RpcUpdateTitle(arenaManager.finalRoundOthers);
			yield return new WaitForSeconds(2);
		}
		//UpdateTitle("");
		RpcUpdateTitle("");
		Robot boss = null;
		List<Robot> otherRobots = new List<Robot>();
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			r.paused = false;
			if (MatchManager.singleton.bossRound) {
				if (r.player.roundWinner >= 2) {
					boss = r;
				} else {
					otherRobots.Add(r);
				}
			}
		}
		// Boss round
		if (MatchManager.singleton.bossRound) {
			RpcUpdateCountdown("");
			while (!boss || boss.health > 0) {
				bool someLeft = false;
				foreach (Robot r in otherRobots) {
					if (r.health > 0) {
						someLeft = true;
						break;
					}
				}
				if (!someLeft) {
					break;
				}
				yield return 0;
			}
			//UpdateTitle(arenaManager.matchOver);
			RpcUpdateTitle(arenaManager.matchOver);
			yield return new WaitForSeconds(5);
			NetworkManager.singleton.ServerChangeScene(GameScenes.MatchResult);
		}
		// Normal round
		while (roundDuration >= 0) {
			roundDuration -= Time.fixedDeltaTime;
			TimeSpan time = TimeSpan.FromSeconds(roundDuration + 1);
			RpcUpdateCountdown(time.ToString(@"mm\:ss"));
			yield return new WaitForFixedUpdate();
		}
		int scoreMax = -1;
		Player roundWinner = null;
		foreach (Player p in FindObjectsOfType<Player>()) {
			p.robot.paused = true;
			if (p.robot.roundScore > scoreMax) {
				scoreMax = p.robot.roundScore;
				roundWinner = p;
			}
		}
		roundWinner.roundWinner++;
		RpcUpdateCountdown("");
		//UpdateTitle(arenaManager.roundWinnerIs.Replace("\\n", "\n").Replace("#", roundWinner.name));
		RpcUpdateTitle(arenaManager.roundWinnerIs.Replace("\\n", "\n").Replace("#", roundWinner.name));
		yield return new WaitForSeconds(5);
		string scene = GameScenes.Auction;
		if (roundWinner.roundWinner >= 2) {
			RpcSetBossRound();
			scene = GameScenes.Arena;
		}
		NetworkManager.singleton.ServerChangeScene(scene);
	}

	[ClientRpc]
	public void RpcSetBossRound() {
		MatchManager.singleton.bossRound = true;
	}

	/*public void UpdateTitle(string s) {
		ArenaManager arenaManager = FindObjectOfType<ArenaManager>();
		if (s == "") {
			arenaManager.title.gameObject.SetActive(false);
			return;
		} else {
			arenaManager.title.gameObject.SetActive(true);
		}
		if (s == arenaManager.finalRoundOthers) {
			foreach (Player player in FindObjectsOfType<Player>()) {
				if (player.robot.isLocalPlayer) {
					if (player.roundWinner >= 2) {
						s = arenaManager.finalRoundBoss;
					}
					break;
				}
			}
		}
		arenaManager.title.text = s;
	}*/
	[ClientRpc]
	public void RpcUpdateTitle(string s) {
		Debug.Log("Title: " + s);
		ArenaManager arenaManager = FindObjectOfType<ArenaManager>();
		/*if (s == "") {
			arenaManager.title.gameObject.SetActive(false);
			return;
		} else {
			arenaManager.title.gameObject.SetActive(true);
		}*/
		if (s == arenaManager.finalRoundOthers) {
			foreach (Player player in FindObjectsOfType<Player>()) {
				if (player.robot.isLocalPlayer) {
					if (player.roundWinner >= 2) {
						s = arenaManager.finalRoundBoss;
					}
					break;
				}
			}
		}
		arenaManager.title.text = s;
	}
	/*[ClientRpc]
	public void RpcTitleSetActive(bool value) {
		FindObjectOfType<ArenaManager>().title.gameObject.SetActive(value);
	}*/
	[ClientRpc]
	public void RpcUpdateCountdown(string s) {
		FindObjectOfType<ArenaManager>().countdown.text = s;
	}

	[Command]
	public void CmdSpawnWorm(GameObject prefab, Vector3 pos) {
		//Debug.Log("Spawn worm");
		StartCoroutine(SpawnWormCoroutine(prefab, pos));
	}

	IEnumerator SpawnWormCoroutine(GameObject prefab, Vector3 pos) {
		yield return new WaitForSeconds(3);
		// hide dust
		//wormHitbox.position = new Vector3(0, -50, 0);

		// spawn the worm
		GameObject worm = Instantiate(prefab, pos, Quaternion.identity);
		NetworkServer.Spawn(worm);
		worm.GetComponent<Rigidbody>().AddForce(0, 50, 0);
		//yield return new WaitForSeconds(3);

		// reposition the dust
		//wormHitbox.position = new Vector3(150, FindObjectOfType<Terrain>().terrainData.GetHeight(150, 150) + 1, 150);
	}
}
