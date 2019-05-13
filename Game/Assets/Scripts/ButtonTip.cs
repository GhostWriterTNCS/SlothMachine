using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ButtonTip : MonoBehaviour {
	public string button;
	Image image;
	Button b;

	void Awake() {
		Refresh();
		b = GetComponentInParent<Button>();
	}

	float freq = 1;
	float temp = 0;
	void Update() {
		if (b) {
			if (button == "Dpad_Left") {
				if (Input.GetAxis("Horizontal") < -0.1) {
					if (temp == 0 || temp > freq) {
						b.onClick.Invoke();
					}
					temp += Time.deltaTime;
				} else {
					temp = 0;
				}
			} else if (button == "Dpad_Right") {
				if (Input.GetAxis("Horizontal") > 0.1) {
					if (temp == 0 || temp > freq) {
						b.onClick.Invoke();
					}
					temp += Time.deltaTime;
				} else {
					temp = 0;
				}
			} else if (Input.GetButton(button)) {
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
