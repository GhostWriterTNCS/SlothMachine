using UnityEngine;

public class KillOnTouch : MonoBehaviour {
	private void OnTriggerExit(Collider other) {
		Robot p = other.GetComponent<Robot>();
		if (p) {
			p.UpdateHealthValue(0);
		}
	}
}
