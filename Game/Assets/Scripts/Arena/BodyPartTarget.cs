using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPartTarget : MonoBehaviour {
	Robot player;
	List<Collider> siblings = new List<Collider>();

	// Start is called before the first frame update
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
				GameObject effect = Instantiate(GetPlayer(other.transform).hitEffect, other.transform);
				effect.transform.localPosition = Vector3.zero;
				player.GetHitted(GetPlayer(other.transform));
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
