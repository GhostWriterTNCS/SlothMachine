using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {
	public float seconds = 3;
	public bool showMinutes = false;
	public UnityEvent onFinish;

	Text text;
	string s;

	void Start() {
		text = GetComponent<Text>();
		if (text) {
			s = text.text;
		}
		StartCoroutine(Run());
	}

	IEnumerator Run() {
		while (seconds >= 0) {
			seconds -= Time.fixedDeltaTime;
			if (text) {
				if (showMinutes) {
					TimeSpan time = TimeSpan.FromSeconds(seconds + 1);
					text.text = s.Replace("#", time.ToString(@"mm\:ss"));
				} else {
					text.text = s.Replace("#", ((int)seconds + 1).ToString());
				}
			}
			yield return new WaitForFixedUpdate();
		}
		if (onFinish != null) {
			onFinish.Invoke();
		}
	}
}
