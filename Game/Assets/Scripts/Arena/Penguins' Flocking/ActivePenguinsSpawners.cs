using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActivePenguinsSpawners : MonoBehaviour {
	public int spawnInterval = 20;

	int spawnTime;
	List<PenguinsSpawner> penguinsSpawner = new List<PenguinsSpawner>();

	void Start() {
		foreach (PenguinsSpawner ps in FindObjectsOfType<PenguinsSpawner>()) {
			penguinsSpawner.Add(ps);
			//ps.gameObject.SetActive(false);
		}
		spawnTime = FindObjectOfType<ArenaManager>().roundDuration / 2;

		StartCoroutine(TimeController());

	}

	IEnumerator TimeController() {
		yield return new WaitForSeconds(spawnTime);
		while (true) {
			foreach (PenguinsSpawner ps in penguinsSpawner) {
				ps.spwanPenguins();
			}
			yield return new WaitForSeconds(spawnInterval);
		}

	}
}
