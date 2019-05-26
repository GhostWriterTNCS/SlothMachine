using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeWheel : MonoBehaviour {
	public Button up;
	public Button upRight;
	public Button right;
	public Button downRight;
	public Button down;
	public Button downLeft;
	public Button left;
	public Button upLeft;

	EventSystem eventSystem;

	void Start() {
		eventSystem = FindObjectOfType<EventSystem>();
	}

	void OnEnable() {
		eventSystem.SetSelectedGameObject(null);
	}

	void Update() {
		if (Input.GetAxis("Camera Vertical") > 0.5f && Input.GetAxis("Camera Horizontal") > 0.5f) { // up right
			eventSystem.SetSelectedGameObject(upRight.gameObject);
		} else if (Input.GetAxis("Camera Vertical") > 0.5f && Input.GetAxis("Camera Horizontal") < -0.5f) { // up left
			eventSystem.SetSelectedGameObject(upLeft.gameObject);
		} else if (Input.GetAxis("Camera Vertical") > 0.5f) { // up
			eventSystem.SetSelectedGameObject(up.gameObject);
		} else if (Input.GetAxis("Camera Vertical") < -0.5f && Input.GetAxis("Camera Horizontal") > 0.5f) { // down right
			eventSystem.SetSelectedGameObject(downRight.gameObject);
		} else if (Input.GetAxis("Camera Vertical") < -0.5f && Input.GetAxis("Camera Horizontal") < -0.5f) { // down left
			eventSystem.SetSelectedGameObject(downLeft.gameObject);
		} else if (Input.GetAxis("Camera Vertical") < -0.5f) { // down
			eventSystem.SetSelectedGameObject(down.gameObject);
		} else if (Input.GetAxis("Camera Horizontal") > 0.5f) { // right
			eventSystem.SetSelectedGameObject(right.gameObject);
		} else if (Input.GetAxis("Camera Horizontal") < -0.5f) { // left
			eventSystem.SetSelectedGameObject(left.gameObject);
		}
	}

	public void Populate() {

	}
}
