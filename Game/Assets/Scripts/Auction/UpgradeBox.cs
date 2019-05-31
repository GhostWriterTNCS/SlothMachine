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
		transform.localScale = Vector3.one;
		StartCoroutine(LoadUpgradeCoroutine());
	}

	public void Refresh() {
		image.sprite = Resources.Load<Sprite>("UI/Upgrades/Permanent/" + level + "_" + ID);
		upgradeName.text = Upgrades.permanent[level][ID].name;
		levelText.text = "Level " + level;
		typeImage.sprite = Resources.Load<Sprite>("UI/Upgrades/Types/" + ((int)Upgrades.permanent[level][ID].type + 1));
		if (description) {
			description.text = Upgrades.permanent[level][ID].description;
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
			AuctionManager auctionManager = FindObjectOfType<AuctionManager>();
			if (auctionManager.upgradesList.transform.childCount > 0) {
				Instantiate(Resources.Load<GameObject>("Prefabs/Separator horizontal"), auctionManager.upgradesList.transform);
			}
			transform.SetParent(auctionManager.upgradesList.transform);
		}
		Refresh();
		RefreshSelected();
	}
}
