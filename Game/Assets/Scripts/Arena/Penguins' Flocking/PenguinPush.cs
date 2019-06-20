using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinPush : MonoBehaviour {
	public float pushForce;

	public void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Robot>()) {
			other.GetComponent<Robot>().CmdAddForce((transform.forward + (transform.up * 0.5f)) * pushForce, ForceMode.Impulse);
		}
	}
}
