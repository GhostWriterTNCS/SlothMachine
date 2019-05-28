using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager audioManager;
	public AudioSource backgroundSource;
	public AudioSource effectSource;
	public AudioClip backgroundMusic;

	private void Awake() {
		if (audioManager == null) {
			audioManager = this;
		} else if (audioManager != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		PlayBackground(audioManager.backgroundMusic);
		//SetSpeakers();
	}

	public void SetSpeakers() {
		/*foreach (AudioSource audioSource in GetComponents<AudioSource>()) {
			audioSource.volume = PlayerStats.Data().audioEnabled ? 1 : 0;
		}*/
	}

	public void PlayClip(AudioClip clip, bool loop = false) {
		if (clip == null) {
			return;
		}
		audioManager.effectSource.clip = clip;
		audioManager.effectSource.loop = loop;
		audioManager.effectSource.Play();
	}

	public void PlayBackground(AudioClip clip) {
		if (clip != null && clip != audioManager.backgroundSource.clip) {
			audioManager.backgroundSource.Stop();
			audioManager.backgroundSource.clip = clip;
			audioManager.backgroundSource.Play();
		}
	}
}
