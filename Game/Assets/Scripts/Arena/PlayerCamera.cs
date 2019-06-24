using UnityEngine;
using UnityEngine.Networking;

public class PlayerCamera : NetworkBehaviour {
	public Camera playerCamera;
	public int maxY = 315;
	public int minY = 30;
	Vector3 cameraOffset;
	Quaternion cameraRotation;
	PlayerMove playerMove;

	void Start() {
		playerMove = GetComponent<PlayerMove>();
		if (!isLocalPlayer) {
			playerCamera.enabled = false;
			playerCamera.GetComponent<AudioListener>().enabled = false;
		}
		//cameraOffset = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y, playerCamera.transform.localPosition.z);
		//cameraRotation = playerCamera.transform.localRotation;
	}

	void Update() {
		if (isLocalPlayer) {
			if (playerMove.canRotateCamera) {
				playerCamera.transform.parent.Rotate(Input.GetAxis("Camera Vertical") * playerMove.turnSpeed, 0, 0);
				if (playerCamera.transform.parent.localRotation.eulerAngles.x > minY && playerCamera.transform.parent.localRotation.eulerAngles.x < 180) {
					playerCamera.transform.parent.localRotation = Quaternion.Euler(minY, 0, 0);
				} else if (playerCamera.transform.parent.localRotation.eulerAngles.x < maxY && playerCamera.transform.parent.localRotation.eulerAngles.x > 180) {
					playerCamera.transform.parent.localRotation = Quaternion.Euler(maxY, 0, 0);
				}
			}
			// Adjust health sliders orientation
			foreach (Robot a in FindObjectsOfType<Robot>()) {
				Canvas c = a.GetComponentInChildren<Canvas>();
				if (c) {
					c.transform.LookAt(playerCamera.transform);
					c.transform.Rotate(0, 180, 0);
				}
			}
		}
	}
}
