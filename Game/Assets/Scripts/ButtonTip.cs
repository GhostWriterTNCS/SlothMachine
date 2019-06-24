using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ButtonTip : MonoBehaviour {
	public string button;
	//public bool capture = true;
	public static float inputInterval = 0.1f;
	public static float threshold = 0.25f;
	Image image;
	Button b;

	void Awake() {
		Refresh();
		b = GetComponentInParent<Button>();
	}

	float inputTimer = 0;
	void Update() {
		if (b) {
			inputTimer -= Time.deltaTime;
			if (button == "Dpad_Left") {
				if (inputTimer <= 0) {
					if (Input.GetAxis("Horizontal") < -threshold) {
						b.onClick.Invoke();
						inputTimer = inputInterval;
					}
				}
			} else if (button == "Dpad_Right") {
				if (inputTimer <= 0) {
					if (Input.GetAxis("Horizontal") > threshold) {
						b.onClick.Invoke();
						inputTimer = inputInterval;
					}
				}
			} else if (Input.GetButtonDown(button)) {
				b.onClick.Invoke();
			}
		}
	}

	public void Refresh() {
		if (!image) {
			image = GetComponent<Image>();
		}
		image.sprite = Resources.Load<Sprite>("Controls\\" + ControllerDetector.controller + "\\" + button);
		image.preserveAspect = true;
	}
}
