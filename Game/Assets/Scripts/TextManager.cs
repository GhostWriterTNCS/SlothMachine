﻿using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextManager : MonoBehaviour {
	public static string fontName = "NovaMono";
	public static int fontSize;
	public static int fontSizeH;
	public static int fontSizeSmall;
	public static Color fontColor = new Color(0.8654326f, 0.9438342f, 0.9811321f);
	public static Color highlightedColor = new Color(0.6630108f, 1, 0.2122642f);
	public static Color backgroundHighlightedColor = new Color(1, 0.9555849f, 0.6367924f, 0.372549f);

	void Awake() {
		fontSize = 34; // Screen.height / 35;
		fontSizeH = 67; // Screen.height / 16;
		fontSizeSmall = 25; // Screen.height / 42;
		foreach (Text t in Resources.FindObjectsOfTypeAll<Text>()) {
			TextProperties tp = t.GetComponent<TextProperties>();
			if (tp) {
				if (tp.type == TextProperties.TextType.Header) {
					t.fontSize = fontSizeH;
					t.resizeTextMaxSize = fontSizeH;
				} else if (tp.type == TextProperties.TextType.Normal) {
					t.fontSize = fontSize;
					t.resizeTextMaxSize = fontSize;
				} else if (tp.type == TextProperties.TextType.Small) {
					t.fontSize = fontSizeSmall;
					t.resizeTextMaxSize = fontSizeSmall;
				} else if (tp.type == TextProperties.TextType.Expand) {
					t.resizeTextMaxSize = 300;
				}
			} else {
				t.fontSize = fontSize;
				t.resizeTextMaxSize = fontSize;
			}
			if (!tp || !tp.useCustomColor) {
				t.color = fontColor;
			}
			if ((!tp || !tp.noShadow) && !t.GetComponent<Shadow>()) {
				Shadow s = t.gameObject.AddComponent<Shadow>();
				if (!tp || (tp && !tp.useCustomShadow)) {
					s.effectDistance = new Vector2(2, -2);
				}
			}
			t.font = (Font)Resources.Load(fontName);
			if (tp && tp.type != TextProperties.TextType.Custom) {
				t.resizeTextForBestFit = true;
			}
#if UNITY_EDITOR
			// Stop execution here when not playing.
			if (!Application.isPlaying) return;
#endif
			t.text = FormatText(t.text);
		}

		foreach (Button b in Resources.FindObjectsOfTypeAll<Button>()) {
			if (b.image.sprite == Resources.Load<Sprite>("UI/button")) {
				b.transition = Selectable.Transition.SpriteSwap;
				SpriteState ss = new SpriteState();
				ss.highlightedSprite = Resources.Load<Sprite>("UI/button-highlighted");
				ss.pressedSprite = Resources.Load<Sprite>("UI/button-pressed");
				ss.disabledSprite = Resources.Load<Sprite>("UI/button-disabled");
				b.spriteState = ss;
			}
		}
	}

	public static string FormatText(string s) {
		s = s.Replace("<h1>", "<size=" + fontSizeH + ">");
		s = s.Replace("</h1>", "</size>");
		return s;
	}
}
