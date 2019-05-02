﻿using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextManager : MonoBehaviour {
	public static string fontName = "NovaMono";
	public static int fontSize;
	public static int fontSizeH;
	public static Color fontColor = new Color(0.8654326f, 0.9438342f, 0.9811321f);

	void Awake() {
		fontSize = Screen.height / 31;
		fontSizeH = Screen.height / 16;
		foreach (Text t in Resources.FindObjectsOfTypeAll<Text>()) {
			TextProperties tp = t.GetComponent<TextProperties>();
			if (tp) {
				if (tp.type == TextProperties.TextType.Header) {
					t.fontSize = fontSizeH;
					t.resizeTextForBestFit = false;
				} else if (tp.type == TextProperties.TextType.Normal) {
					t.fontSize = fontSize;
					t.resizeTextForBestFit = false;
				} else if (tp.type == TextProperties.TextType.Expand) {
					t.resizeTextForBestFit = true;
					t.resizeTextMaxSize = 300;
				}
			} else {
				t.fontSize = fontSize;
			}
			if (!tp || !tp.useCustomColor) {
				t.color = fontColor;
			}
			t.font = (Font)Resources.Load(fontName);
#if UNITY_EDITOR
			// Stop execution here when not playing.
			if (!Application.isPlaying) return;
#endif
			t.text = t.text.Replace("<h1>", "<size=" + fontSizeH + ">");
			t.text = t.text.Replace("</h1>", "</size>");
		}
	}
}
