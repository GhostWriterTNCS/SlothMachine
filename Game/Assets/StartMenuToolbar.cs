using UnityEngine;

public class StartMenuToolbar : MonoBehaviour {
	public GameObject[] panels;

	int index = 0;

	public void showPanel() {
		foreach (GameObject go in panels) {
			go.SetActive(false);
		}
		panels[index].SetActive(true);
	}
	public void showPrevious() {
		index--;
		if (index < 0) {
			index = panels.Length - 1;
		}
		showPanel();
	}
	public void showNext() {
		index++;
		if (index >= panels.Length) {
			index = 0;
		}
		showPanel();
	}
}
