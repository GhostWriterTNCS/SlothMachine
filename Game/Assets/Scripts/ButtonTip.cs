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

	void Update() {
		if (b) {
			if (Input.GetButton(button)) {
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
