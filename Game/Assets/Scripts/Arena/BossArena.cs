using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour {
	public Transform bossSpawnPosition;

	// Start is called before the first frame update
	void Start() {
		FindObjectOfType<ArenaManager>().arenaReady = true;
	}

	// Update is called once per frame
	void Update() {

	}
}
