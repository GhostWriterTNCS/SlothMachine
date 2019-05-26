using System.Collections.Generic;
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
	[Space]
	public Button[] buttons;
	public GameObject center;
	public Text upgradeName;
	public Text upgradeDesc;
	public Text upgradePrice;
	public Button upgradeBuy;

	public Player player;
	EventSystem eventSystem;
	List<int> upgrades = new List<int>();

	void Start() {
		eventSystem = FindObjectOfType<EventSystem>();
		Populate();
	}

	void OnEnable() {
		center.SetActive(false);
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
		upgrades.Clear();
		for (int i = 0; i < buttons.Length; i++) {
			int u = 0;
			do {
				u = Random.Range(1, Upgrades.temporary.Length);
			} while (upgrades.Contains(u));
			upgrades.Add(u);
			buttons[i].GetComponent<UpgradeWheelSegment>().upgrade = Upgrades.temporary[u];
			buttons[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Upgrades/" + u);
		}
	}

	public void ShowDetails(Upgrade u) {
		upgradeName.text = u.name;
		upgradeDesc.text = u.description;
		upgradePrice.text = u.price + " scraps";
		upgradeBuy.onClick.AddListener(() => { });
		center.SetActive(true);
	}
}
