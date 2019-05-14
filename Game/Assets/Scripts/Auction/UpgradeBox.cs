using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpgradeBox : NetworkBehaviour {
	public Image image;
	public Text upgradeName;
	public Text levelText;
	public Image typeImage;
	public Text description;
	public Image backgroundImage;

	[SyncVar]
	public int ID;
	[SyncVar]
	public int level;
	[SyncVar]
	public bool selected;
	[SyncVar]
	public bool isUpdated;

	void Start() {
		isUpdated = true;
		StartCoroutine(LoadUpgradeCoroutine());
	}

	public void Refresh() {
		image.sprite = Resources.Load<Sprite>("UI/Upgrades/" + level + "_" + ID);
		upgradeName.text = Upgrades.list[level][ID].name;
		levelText.text = "Level " + level;
		Debug.Log("UI/Upgrades/Types/" + ((int)Upgrades.list[level][ID].type + 1));
		typeImage.sprite = Resources.Load<Sprite>("UI/Upgrades/Types/" + ((int)Upgrades.list[level][ID].type + 1));
		if (description) {
			description.text = Upgrades.list[level][ID].description;
		}
	}

	public void RefreshSelected() {
		StartCoroutine(RefreshSelectedCoroutine());
	}

	IEnumerator RefreshSelectedCoroutine() {
		while (!isUpdated) {
			yield return new WaitForSeconds(0.05f);
		}
		if (backgroundImage) {
			backgroundImage.enabled = selected;
		}
		if (selected) {
			UpgradeBox current = FindObjectOfType<AuctionManager>().currentUpgrade;
			current.ID = ID;
			current.level = level;
			current.Refresh();
		}
		isUpdated = false;
	}

	IEnumerator LoadUpgradeCoroutine() {
		while (ID == 0) {
			yield return new WaitForSeconds(0.05f);
		}
		if (transform.parent == null) {
			transform.SetParent(FindObjectOfType<AuctionManager>().upgradesList.transform);
		}
		Refresh();
		RefreshSelected();
	}
}
