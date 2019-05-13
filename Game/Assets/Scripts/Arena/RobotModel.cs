using UnityEngine;
using UnityEngine.Networking;

public class RobotModel : NetworkBehaviour {
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
