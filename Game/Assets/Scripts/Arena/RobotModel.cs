using UnityEngine;
using UnityEngine.Networking;

public class RobotModel : NetworkBehaviour {
	[Range(1, 5)]
	public int health = 3;
	[Range(1, 5)]
	public int attack = 3;
	[Range(1, 5)]
	public int defense = 3;
	[Range(1, 5)]
	public int speed = 3;

	[Space]
	public RuntimeAnimatorController animatorController;
	public Avatar avatar;

	public Collider leftHand;
	public Collider rightHand;

	public Collider leftFoot;
	public Collider rightFoot;

	public Collider head;
}
