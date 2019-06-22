using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinDestroy : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PenguinsBlending>()) {
			Destroy(other.gameObject);
		}
	}
}
