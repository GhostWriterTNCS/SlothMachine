using UnityEngine;

public class SetHandsParticle : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		Robot p = other.GetComponent<Robot>();
		if (p && transform.childCount > 0) {
			p.SetUpgradeParticle(transform.GetChild(0).gameObject);
		}
	}
}
