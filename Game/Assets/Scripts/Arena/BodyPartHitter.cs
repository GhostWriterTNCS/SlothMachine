using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPartHitter : MonoBehaviour {
	public Robot robot;
	public List<Collider> hitters = new List<Collider>();
	List<Collider> siblings = new List<Collider>();

	void Start() {
		robot = GetRobot(transform);
		GetComponent<Collider>().isTrigger = true;

		Transform t = transform.parent;
		while (t.parent) {
			t = t.parent;
		}
		siblings.AddRange(t.GetComponentsInChildren<Collider>());
	}

	private void OnTriggerEnter(Collider other) {
		if (other.GetComponent<BodyPartTarget>()) {
			if (!siblings.Contains(other) && !hitters.Contains(other) && other.isTrigger) {
				hitters.Add(other);
				GetRobot(other.transform).CmdGetHitted(robot.gameObject, transform.position);
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
