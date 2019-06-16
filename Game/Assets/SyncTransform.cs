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
			CmdSendValues(transform.position, transform.rotation);
		} else {
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * transitionSpeed);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * transitionSpeed);
		}
	}

	void CmdSendValues(Vector3 newPos, Quaternion newRot) {
		position = newPos;
		rotation = newRot;
	}
}
