using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ButtonTip : MonoBehaviour {
	public string button;
	Image image;

	void Awake() {
		Refresh();
	}

	public void Refresh() {
		if (!image) {
			image = GetComponent<Image>();
		}
		image.sprite = Resources.Load<Sprite>("Controls\\" + InputDetector.controller + "\\" + button);
		image.preserveAspect = true;
	}
}
