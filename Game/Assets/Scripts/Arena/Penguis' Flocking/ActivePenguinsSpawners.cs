﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActivePenguinsSpawners : MonoBehaviour {
	private float timerArena;
	private int spawnTime;
	public List<PenguinsSpawner> penguinsSpawner;
	// Start is called before the first frame update
	void Start() {
		foreach (PenguinsSpawner ps in FindObjectsOfType<PenguinsSpawner>()) {
			penguinsSpawner.Add(ps);
			//ps.gameObject.SetActive(false);
		}
		spawnTime = FindObjectOfType<ArenaManager>().roundDuration / 2;

		StartCoroutine(TimeController());

	}




	// Update is called once per frame
	IEnumerator TimeController() {
#if UNITY_EDITOR
		NetworkArenaManager NAM = FindObjectOfType<NetworkArenaManager>();
		while (NAM.roundDuration >= spawnTime) {
			yield return new WaitForSeconds(1);
		}
#else
			yield return new WaitForSeconds(spawnTime);
#endif
		while (true) {
			foreach (PenguinsSpawner ps in penguinsSpawner) {
				ps.spwanPenguins();
			}
			yield return new WaitForSeconds(20);
		}

	}
}
