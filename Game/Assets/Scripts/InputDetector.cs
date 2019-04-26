using UnityEngine;

public class InputDetector : MonoBehaviour {
	public static string controller = "Xbox One";
	void Update() {
		if (Input.GetJoystickNames().Length > 0) {
			if (Input.GetJoystickNames()[0].ToLower().Contains("xbox 360") && controller != "Xbox 360") {
				controller = "Xbox 360";
				foreach (ButtonTip bt in FindObjectsOfType<ButtonTip>()) {
					bt.Refresh();
				}
			} else if (Input.GetJoystickNames()[0].ToLower().Contains("ps4") && controller != "PS4") {
				controller = "PS4";
				foreach (ButtonTip bt in FindObjectsOfType<ButtonTip>()) {
					bt.Refresh();
				}
			}
		}
	}
}
