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

	public void LoadUpgrade() {
		StartCoroutine(LoadUpgradeCoroutine());
	}
	IEnumerator LoadUpgradeCoroutine() {
		while (ID == 0) {
			yield return new WaitForSeconds(0.01f);
		}
		transform.SetParent(FindObjectOfType<AuctionManager>().upgradesList.transform);
		image.sprite = Resources.Load<Sprite>("UI/Upgrades/" + level + "_" + ID);
		upgradeName.text = Upgrades.list[level][ID].name;
		levelText.text = "Level " + level;
		if (description)
			description.text = Upgrades.list[level][ID].description;
	}
}
