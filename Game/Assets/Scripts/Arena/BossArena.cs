using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour {
	public Transform bossSpawnPosition;

	void Start() {
		FindObjectOfType<ArenaManager>().arenaReady = true;
	}
}
