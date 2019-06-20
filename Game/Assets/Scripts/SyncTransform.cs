using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2)]
public class SyncTransform : NetworkBehaviour {
	public float transitionSpeed = 5;
	public float snapThreshold = 15;
	public float sendRate = 0.1f;
	[SyncVar]
	public Vector3 position;
	[SyncVar]
	public Quaternion rotation;
	/*[SyncVar]
	public bool respawn;*/

	//[SyncVar]
	bool cmdEnabled = false;
	PlayerMove playerMove;

	void Start() {
		if (isLocalPlayer) {
			CmdEnable(true);
		}
		playerMove = GetComponent<PlayerMove>();
	}

	//[Command]
	public void CmdEnable(bool value) {
		cmdEnabled = value;
		if (cmdEnabled) {
			StartCoroutine(SetValuesCoroutine());
		}
	}

	IEnumerator SetValuesCoroutine() {
		while (true) {
			if (cmdEnabled/*&& !respawn*/) {
				//Debug.Log("My position is " + transform.position);
				CmdSetValues(transform.position, transform.rotation);
				yield return new WaitForSeconds(sendRate);
			} else {
				yield break;
			}
		}
	}

	void Update() {
		if (!cmdEnabled) {
			float dist = Vector3.Distance(transform.position, position);
			if (dist > snapThreshold) {
				transform.position = position;
			} else {
				transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * playerMove.moveSpeed * (dist < 0.5f ? dist * 2 : 1));
			}
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * playerMove.moveSpeed);
			//respawn = false;
		}
	}

	[Command]
	public void CmdSetValues(Vector3 newPos, Quaternion newRot) {
		position = newPos;
		rotation = newRot;
	}
}
