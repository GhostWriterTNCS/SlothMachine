using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeWheelSegment : MonoBehaviour, ISelectHandler {
	public Upgrade upgrade;
	UpgradeWheel upgradeWheel;

	void Start() {
		upgradeWheel = FindObjectOfType<ArenaManager>().upgradeWheel;
	}

	public void OnSelect(BaseEventData eventData) {
		upgradeWheel.ShowDetails(upgrade);
	}
}
