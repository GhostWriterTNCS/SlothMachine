using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	[SyncVar]
	public string robotModel = "Dozzer";
	public static Player singleton;

	void Awake() {
		if (singleton == null) {
			singleton = this;
		} else {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}
}
