using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextBlinking : MonoBehaviour {
	public Color altColor;
	public float interval = 0.5f;

	Color baseColor;
	Text text;
	int spawnTime;

	void Start() {
		text = GetComponent<Text>();
		baseColor = text.color;
		spawnTime = FindObjectOfType<ArenaManager>().roundDuration / 2;
		StartCoroutine(Refresh());
	}

	IEnumerator Refresh() {
		yield return new WaitForSeconds(5);
		while (!FindObjectOfType<NetworkArenaManager>()) {
			yield return 0;
		}
		while (FindObjectOfType<NetworkArenaManager>().roundDuration > spawnTime) {
			yield return new WaitForSeconds(1);
		}
		while (true) {
			Debug.Log(altColor);
			text.color = altColor;
			yield return new WaitForSeconds(interval);
			Debug.Log(baseColor);
			text.color = baseColor;
			yield return new WaitForSeconds(interval);
		}
	}
}
