using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager singleton;
	public AudioSource backgroundSource;
	public AudioSource effectSource;
	public AudioClip backgroundMusic;

	private void Awake() {
		if (singleton == null) {
			singleton = this;
		} else if (singleton != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		PlayBackground(singleton.backgroundMusic);
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
		singleton.effectSource.clip = clip;
		singleton.effectSource.loop = loop;
		singleton.effectSource.Play();
	}

	public void PlayBackground(AudioClip clip) {
		if (clip != null && clip != singleton.backgroundSource.clip) {
			singleton.backgroundSource.Stop();
			singleton.backgroundSource.clip = clip;
			singleton.backgroundSource.Play();
		}
	}
}
