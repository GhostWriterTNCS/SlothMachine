using UnityEngine;
using UnityEngine.UI;

public class StartMenuToolbar : MonoBehaviour {
	public GameObject[] panels;
	public GameObject[] buttons;

	int index = 0;

	void Start() {
		showPanel();
	}

	public void showPanel() {
		foreach (GameObject go in panels) {
			go.SetActive(false);
		}
		foreach (GameObject go in buttons) {
			go.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
		}
		panels[index].SetActive(true);
		buttons[index].GetComponent<Image>().color = Color.white;
	}

	public void showPrevious() {
		Debug.Log("Show previous");
		index--;
		if (index < 0) {
			index = panels.Length - 1;
		}
		showPanel();
	}

	public void showNext() {
		Debug.Log("Show next");
		index++;
		if (index >= panels.Length) {
			index = 0;
		}
		showPanel();
	}
}
