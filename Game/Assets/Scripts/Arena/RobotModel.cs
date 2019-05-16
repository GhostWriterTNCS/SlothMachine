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
	[SerializeField]
	public RuntimeAnimatorController animatorController;
	[SerializeField]
	public Avatar avatar;

	[SerializeField]
	public SphereCollider leftHand;
	[SerializeField]
	public SphereCollider rightHand;

	[SerializeField]
	public SphereCollider leftFoot;
	[SerializeField]
	public SphereCollider rightFoot;
}
