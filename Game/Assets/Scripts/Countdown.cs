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
			if (text) {
				if (showMinutes) {
					TimeSpan time = TimeSpan.FromSeconds(seconds);
					text.text = s.Replace("#", time.ToString(@"m\:ss"));
				} else {
					text.text = s.Replace("#", ((int)seconds).ToString());
				}
			}
			seconds -= Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		if (onFinish != null) {
			onFinish.Invoke();
		}
	}
}
