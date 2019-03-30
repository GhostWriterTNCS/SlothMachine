using UnityEngine;

public class SetMaterial : MonoBehaviour {
	public Material material;

	private void OnTriggerEnter(Collider other) {
		SkinnedMeshRenderer smr = other.GetComponent<SkinnedMeshRenderer>();
		if (smr) {
			smr.material = material;
			Destroy(gameObject);
		}
	}
}
