using UnityEngine;

public class SetHandsParticle : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		Player p = other.GetComponent<Player>();
		if (p && transform.childCount > 0) {
			p.SetHandsParticle(transform.GetChild(0).gameObject);
		}
	}
}
