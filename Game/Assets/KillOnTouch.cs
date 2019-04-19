using UnityEngine;

public class KillOnTouch : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		Player p = other.GetComponent<Player>();
		if (p) {
			p.UpdateHealthValue(0);
		}
	}
}
