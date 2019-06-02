using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormHitbox : MonoBehaviour {
	public void OnTriggerEnter(Collider other) {
		if (other.GetComponent<BodyPartTarget>()) {
			Debug.Log(other.name);
			Robot robot = other.GetComponent<BodyPartTarget>().robot;
			if (robot) {
				robot.UpdateHealth(-robot.healthMax);
			}
		}
	}
}
