using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnPoint : MonoBehaviour {
	public bool busy;

	void Start() {
		busy = false;
	}

	void Update() {
		busy = false;
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			if (Vector3.Distance(transform.position, r.transform.position) < 4) {
				busy = true;
				return;
			}
		}
	}
}
