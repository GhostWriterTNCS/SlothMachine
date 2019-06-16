using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTransform : NetworkBehaviour {
	public float transitionSpeed = 0.1f;
	[SyncVar]
	Vector3 position;
	[SyncVar]
	Quaternion rotation;

	void Update() {
		if (isLocalPlayer) {
			CmdSendValues();
		} else {
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * transitionSpeed);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, transitionSpeed);
		}
	}

	void CmdSendValues() {
		position = transform.position;
		rotation = transform.rotation;
	}
}
