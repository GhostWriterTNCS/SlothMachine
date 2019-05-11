﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : NetworkBehaviour {
	public float moveSpeed = 3;
	public float turnSpeed = 3;
	public bool canMove = true;

	Rigidbody rigidbody;
	Animator animator;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
	}

	void Update() {
		if (isLocalPlayer && canMove) {
			// Move player and rotate camera
			rigidbody.MovePosition(rigidbody.position + (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed);
			rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(Vector3.up * Input.GetAxis("Camera Horizontal") * turnSpeed));
			animator.SetFloat("WalkH", Input.GetAxis("Horizontal"));
			animator.SetFloat("WalkV", Input.GetAxis("Vertical"));
		} else {
			animator.SetFloat("WalkH", 0);
			animator.SetFloat("WalkV", 0);
		}
	}
}