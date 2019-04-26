using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {
	public static int fontSize;
	public static int fontSizeH;

	// Start is called before the first frame update
	void Awake() {
		if (fontSize == 0) {
			fontSize = Screen.height / 43;
		}
		if (fontSizeH == 0) {
			fontSizeH = Screen.height / 26;
		}
		foreach (Text t in FindObjectsOfType<Text>()) {
			if (t.GetComponent<TextHeader>()) {
				t.fontSize = fontSizeH;
			} else {
				t.fontSize = fontSize;
			}
		}
	}

	// Update is called once per frame
	void Update() {

	}
}
