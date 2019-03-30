using System.Collections.Generic;
using UnityEngine;

public class Hitted : MonoBehaviour {
	public Animator animator;
	MyAnimator myAnimator;
	List<Collider> siblings = new List<Collider>();

	// Start is called before the first frame update
	void Start() {
		myAnimator = animator.GetComponent<MyAnimator>();

		Transform t = transform.parent;
		while (t.parent) {
			t = t.parent;
		}
		siblings.AddRange(t.GetComponentsInChildren<Collider>());
	}

	// Update is called once per frame
	void Update() {

	}

	private void OnTriggerEnter(Collider other) {
		if (animator && !siblings.Contains(other) && other.isTrigger && !other.GetComponent<Hitted>()) {
			Debug.Log(name + " collided by " + other.name);
			other.enabled = false;
			animator.SetTrigger("Reaction");

			myAnimator.UpdateHealth(-5);
		}
	}
}
