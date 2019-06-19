using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WormDestroyer : MonoBehaviour {
	public void OnTriggerEnter(Collider other) {
		if (other.GetComponent<WormHitbox>()) {
			//Destroy(other.gameObject);
			other.gameObject.SetActive(false);
		}
	}
}
