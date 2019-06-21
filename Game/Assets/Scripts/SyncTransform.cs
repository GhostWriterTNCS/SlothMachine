using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2)]
public class SyncTransform : NetworkBehaviour {
	public float snapThreshold = 15;
	//public float sendRate = 0.2f;
	[SyncVar]
	public Vector3 position;
	[SyncVar]
	public Quaternion rotation;

	bool cmdEnabled = false;
	PlayerMove playerMove;

	void Start() {
		if (isLocalPlayer) {
			EnableSetValues(true);
		}
		playerMove = GetComponent<PlayerMove>();
	}

	public void EnableSetValues(bool value) {
		cmdEnabled = value;
		/*if (cmdEnabled) {
			StartCoroutine(SetValuesCoroutine());
		}*/
	}

	/*IEnumerator SetValuesCoroutine() {
		while (true) {
			if (cmdEnabled) {
				//Debug.Log("My position is " + transform.position);
				if (position != transform.position) {
					CmdSetPosition(transform.position);
				}
				if (rotation != transform.rotation) {
					CmdSetRotation(transform.rotation);
				}
				yield return new WaitForSeconds(sendRate);
			} else {
				yield break;
			}
		}
	}*/

	void Update() {
		if (cmdEnabled) {
			//Debug.Log("My position is " + transform.position);
			if (position != transform.position) {
				CmdSetPosition(transform.position);
			}
			if (rotation != transform.rotation) {
				CmdSetRotation(transform.rotation);
			}
		} else { //if (!cmdEnabled) {
			float dist = Vector3.Distance(transform.position, position);
			if (dist > snapThreshold) {
				transform.position = position;
			} else {
				transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * playerMove.moveSpeed); //* (dist < 0.5f ? dist * 2 : 1));
			}
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * playerMove.moveSpeed);
		}
	}

	[Command]
	public void CmdSetPosition(Vector3 pos) {
		position = pos;
	}
	[Command]
	public void CmdSetRotation(Quaternion rot) {
		rotation = rot;
	}
}
