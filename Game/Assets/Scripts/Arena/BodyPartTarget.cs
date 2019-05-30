using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPartTarget : MonoBehaviour {
	public Robot robot;
	List<Collider> siblings = new List<Collider>();

	void Start() {
		robot = GetRobot(transform);

		Transform t = transform.parent;
		while (t.parent) {
			t = t.parent;
		}
		siblings.AddRange(t.GetComponentsInChildren<Collider>());
	}

	private void OnTriggerEnter(Collider other) {
		if (other.GetComponent<BodyPartHitter>() != null) {
			if (!siblings.Contains(other) && other.isTrigger && !other.GetComponent<BodyPartTarget>()) {
				other.enabled = false;
				robot.CmdGetHitted(GetRobot(other.transform).gameObject, other.transform.position);
			}
		}
	}

	Robot GetRobot(Transform t) {
		while (t != null && t.GetComponentInParent<Robot>() == null) {
			t = t.parent;
		}
		return t.GetComponentInParent<Robot>();
	}
}
