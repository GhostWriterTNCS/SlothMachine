using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeWheelSegment : MonoBehaviour, ISelectHandler {
	public int upgradeID;
	public int position;
	UpgradeWheel upgradeWheel;

	void Start() {
		upgradeWheel = FindObjectOfType<ArenaManager>().upgradeWheel;
	}

	public void OnSelect(BaseEventData eventData) {
		upgradeWheel.ShowDetails(upgradeID, position);
	}
}
