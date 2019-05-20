using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPartTarget : MonoBehaviour {
	Robot player;
	List<Collider> siblings = new List<Collider>();

	void Start() {
		player = GetPlayer(transform);

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
				player.CmdGetHitted(GetPlayer(other.transform), other.transform.position);
			}
		}
	}

	Robot GetPlayer(Transform t) {
		while (transform != null && transform.GetComponentInParent<Robot>() == null) {
			t = transform.parent;
		}
		return transform.GetComponentInParent<Robot>();
	}
}
