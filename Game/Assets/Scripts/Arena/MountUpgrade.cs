using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MountUpgrade : NetworkBehaviour {
	[SyncVar]
	public byte type;
	[SyncVar]
	public byte ID;
	[SyncVar]
	public GameObject robotGO;
	public Robot robot;

	void Start() {
		StartCoroutine(Load());
	}

	IEnumerator Load() {
		while (!robotGO) {
			yield return 0;
		}
		robot = robotGO.GetComponent<Robot>();
		switch (Upgrades.permanent[type][ID].type) {
			case UpgradeTypes.Hands:
				transform.SetParent(robot.rightHand.transform);
				break;
			case UpgradeTypes.Feet:
				transform.SetParent(robot.rightFoot.transform);
				break;
			case UpgradeTypes.Armor:
				transform.SetParent(robot.body.transform);
				break;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one / robot.transform.localScale.x;
	}
}
