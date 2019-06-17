using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTransform : NetworkBehaviour {
	public float transitionSpeed = 5;
	public float snapThreshold = 15;
	[SyncVar]
	public Vector3 position;
	[SyncVar]
	public Quaternion rotation;

	bool cmdEnabled = false;

	void Start() {
		if (isLocalPlayer) {
			cmdEnabled = true;
		}
	}

	[Command]
	public void CmdEnable(bool value) {
		cmdEnabled = value;
	}

	void Update() {
		if (cmdEnabled) {
			//Debug.Log("My position is " + transform.position);
			CmdSetValues(transform.position, transform.rotation);
		} else {
			if (Vector3.Distance(transform.position, position) > snapThreshold) {
				transform.position = position;
			} else {
				transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * transitionSpeed);
			}
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * transitionSpeed);
		}
	}

	[Command]
	void CmdSetValues(Vector3 newPos, Quaternion newRot) {
		position = newPos;
		rotation = newRot;
	}
}
