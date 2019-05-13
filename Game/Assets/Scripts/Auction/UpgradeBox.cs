using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpgradeBox : NetworkBehaviour {
	public Image image;
	public Text upgradeName;
	public Text levelText;
	public Text description;
	public Image backgroundImage;

	[SyncVar]
	public int ID;
	[SyncVar]
	public int level;
	[SyncVar]
	public bool selected;

	void Start() {
		StartCoroutine(LoadUpgradeCoroutine());
	}

	public void Refresh() {
		image.sprite = Resources.Load<Sprite>("UI/Upgrades/" + level + "_" + ID);
		upgradeName.text = Upgrades.list[level][ID].name;
		levelText.text = "Level " + level;
		if (description) {
			description.text = Upgrades.list[level][ID].description;
		}
	}

	public void RefreshSelected() {
		if (backgroundImage) {
			backgroundImage.enabled = selected;
		}
		if (selected) {
			UpgradeBox current = FindObjectOfType<AuctionManager>().currentUpgrade;
			current.ID = ID;
			current.level = level;
			current.Refresh();
		}
	}

	IEnumerator LoadUpgradeCoroutine() {
		while (ID == 0) {
			yield return new WaitForSeconds(0.01f);
		}
		Debug.Log("UpgradeBox loading");
		if (transform.parent == null) {
			transform.SetParent(FindObjectOfType<AuctionManager>().upgradesList.transform);
		}
		Refresh();
		RefreshSelected();
	}
}
