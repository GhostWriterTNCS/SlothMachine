using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Countdown : MonoBehaviour {
	public float seconds = 3;

	Text text;
	string s;

	void Start() {
		text = GetComponent<Text>();
		s = text.text;
	}

	void Update() {
		text.text = s.Replace("#", ((int)seconds).ToString());
		seconds -= Time.deltaTime;
	}
}
