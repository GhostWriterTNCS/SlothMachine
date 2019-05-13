using UnityEngine;

public class SwitchGameObjects : MonoBehaviour {
	public GameObject gameObject1;
	public GameObject gameObject2;

	void Start() {
		gameObject1.SetActive(true);
		gameObject2.SetActive(false);
	}

	public void Switch() {
		bool b = gameObject1.activeSelf;
		gameObject1.SetActive(!b);
		gameObject2.SetActive(b);
	}
}
