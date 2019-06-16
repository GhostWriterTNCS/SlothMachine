using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncTransform : NetworkBehaviour {
	public float transitionSpeed = 0.1f;
	[SyncVar]
	public Vector3 position;
	[SyncVar]
	public Quaternion rotation;

	void Update() {
		if (isLocalPlayer || GetComponent<Robot>() != null && GetComponent<Robot>().player.isAgent) {
			Debug.Log("My position is " + transform.position);
			CmdSendValues(transform.position, transform.rotation);
		} else {
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * transitionSpeed);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * transitionSpeed);
		}
	}

	[Command]
	void CmdSendValues(Vector3 newPos, Quaternion newRot) {
		position = newPos;
		rotation = newRot;
	}
}
