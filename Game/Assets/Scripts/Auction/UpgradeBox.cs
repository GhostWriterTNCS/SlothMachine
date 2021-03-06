﻿using System.Collections;
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
	public byte ID;
	[SyncVar]
	public byte level;
	[SyncVar]
	public bool selected;
	[SyncVar]
	public bool isUpdated;
	[SyncVar]
	public bool isIntro;

	void Start() {
		if (backgroundImage) {
			backgroundImage.color = TextManager.backgroundHighlightedColor;
		}
		FindObjectOfType<AuctionManager>().StartCoroutine(LoadUpgradeCoroutine());
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
		FindObjectOfType<AuctionManager>().StartCoroutine(RefreshSelectedCoroutine());
	}

	IEnumerator RefreshSelectedCoroutine() {
		/*if (isIntro) {
			yield break;
		}*/
		while (!isUpdated) {
			yield return 0;
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
		//Debug.Log(name + " refreshed.");
	}

	IEnumerator LoadUpgradeCoroutine() {
		while (ID == 0) {
			yield return 0;
		}
		while (FindObjectOfType<Canvas>().transform.localScale.x < 0.1f) {
			yield return 0;
		}
		if (isIntro || !description) {
			transform.localScale = FindObjectOfType<Canvas>().transform.localScale;
		}
		if (transform.parent == null) {
			AuctionManager auctionManager = FindObjectOfType<AuctionManager>();
			if (isIntro) {
				transform.SetParent(auctionManager.introPanel.GetComponentInChildren<GridLayoutGroup>().transform);
			} else {
				if (auctionManager.upgradesList.transform.childCount > 0) {
					Instantiate(Resources.Load<GameObject>("Prefabs/Separator horizontal"), auctionManager.upgradesList.transform);
				}
				transform.SetParent(auctionManager.upgradesList.transform);
			}
		}
		isUpdated = true;
		Refresh();
		RefreshSelected();
	}
}
