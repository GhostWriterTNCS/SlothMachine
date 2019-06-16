using UnityEngine;

public class KillOnTouch : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		Robot p = other.GetComponent<Robot>();
		if (p && p.player) {
			p.UpdateHealth(-p.healthMax);
		}
	}
}
