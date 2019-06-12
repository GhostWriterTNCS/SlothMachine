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
	System.Action currentAction;

	void Start() {
		if (!eventSystem) {
			eventSystem = FindObjectOfType<EventSystem>();
		}
		Populate();
	}

	void OnEnable() {
		center.SetActive(false);
		if (!eventSystem) {
			eventSystem = FindObjectOfType<EventSystem>();
		}
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
		if (Input.GetButtonDown("A")) {
			currentAction();
			//gameObject.SetActive(false);
		}
	}

	public void Populate() {
		upgrades.Clear();
		for (int i = 0; i < buttons.Length; i++) {
			AddNew(i);
		}
	}

	public void AddNew(int position) {
		int u = 0;
		do {
			u = Random.Range(1, Upgrades.temporary.Length);
		} while (upgrades.Contains(u));
		upgrades.Add(u);
		buttons[position].GetComponent<UpgradeWheelSegment>().upgradeID = u;
		buttons[position].GetComponent<UpgradeWheelSegment>().position = position;
		buttons[position].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Upgrades/Temporary/" + u);
	}

	public void ShowDetails(int ID, int position) {
		Upgrade u = Upgrades.temporary[ID];
		upgradeName.text = u.name;
		upgradeDesc.text = u.description;
		upgradePrice.text = u.price + " scraps";
		currentAction = () => { player.CmdAddTemporaryUpgrade(ID); upgrades[position] = 0; AddNew(position); };
		center.SetActive(true);
	}
}
