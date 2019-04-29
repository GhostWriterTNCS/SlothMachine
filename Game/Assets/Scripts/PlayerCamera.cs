﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerCamera : NetworkBehaviour {
	public Camera playerCamera;
	Vector3 cameraOffset;
	Quaternion cameraRotation;

	void Start() {
		if (!isLocalPlayer) {
			playerCamera.enabled = false;
			playerCamera.GetComponent<AudioListener>().enabled = false;
		}
		cameraOffset = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y, playerCamera.transform.localPosition.z);
		cameraRotation = playerCamera.transform.localRotation;
	}

	void Update() {
		if (isLocalPlayer) {
			// Adjust health sliders orientation
			foreach (Robot a in FindObjectsOfType<Robot>()) {
				a.GetComponentInChildren<Canvas>().transform.LookAt(playerCamera.transform);
			}
		}
	}
}
