﻿using System.Collections.Generic;
using UnityEngine;

public class Hitted : MonoBehaviour {
	public Animator animator;
	Player player;
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
		if (animator && !siblings.Contains(other) && other.isTrigger && !other.GetComponent<Hitted>()) {
			//other.enabled = false;
			player.GetHitted(GetPlayer(other.transform));
		}
	}

	Player GetPlayer(Transform t) {
		while (transform != null && transform.GetComponentInParent<Player>() == null) {
			t = transform.parent;
		}
		return transform.GetComponentInParent<Player>();
	}
}
