using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	public bool IsFree() {
		foreach (Robot r in FindObjectsOfType<Robot>()) {
			if (Vector3.Distance(transform.position, r.transform.position) < 4) {
				return false;
			}
		}
		return true;
	}
}
