﻿using UnityEngine;
using UnityEngine.Networking;

public class RobotModelPerTrailer : MonoBehaviour {
	[Range(1, 9)]
	public int health = 3;
	[Range(1, 9)]
	public int attack = 3;
	[Range(1, 9)]
	public int defense = 3;
	[Range(1, 9)]
	public int speed = 3;

	[Space]
	public RuntimeAnimatorController animatorController;
	public Avatar avatar;

	public Collider leftHand;
	public Collider rightHand;

	public Collider leftFoot;
	public Collider rightFoot;

	public Collider head;
	public Collider body;
}