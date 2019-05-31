using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudio : MonoBehaviour {
	public AudioClip backgroundMusic;
	public AudioClip[] randomSounds;

	void Start() {
		AudioManager.singleton.PlayBackground(backgroundMusic);
		if (randomSounds.Length > 0) {
			StartCoroutine(RandomSoundsCoroutine());
		}
	}

	IEnumerator RandomSoundsCoroutine() {
		AudioManager.singleton.PlayClip(randomSounds[Random.Range(0, randomSounds.Length)]);
		yield return new WaitForSeconds(Random.Range(5, 10));
	}
}
