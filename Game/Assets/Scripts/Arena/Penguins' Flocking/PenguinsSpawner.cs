using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinsSpawner : MonoBehaviour {
	public GameObject penguin = null;
	public float radius = 10f;
	public int count = 30;
	public int jumpForce = 600;

	public void spwanPenguins() {
		//Random.InitState((int)FindObjectOfType<NetworkArenaManager>().roundDuration);
		Random.InitState(0);
		if (penguin != null) {
			for (int i = 0; i < count; i += 1) {
				GameObject go = Instantiate(penguin, transform.position + Random.insideUnitSphere * radius, Quaternion.Euler(0, Random.Range(0, 359), 0));
				go.name = penguin.name + " " + i;
				go.GetComponentInChildren<MeshRenderer>().transform.Rotate(90, 0, 0);
				go.transform.LookAt(transform.position + Random.insideUnitSphere * radius);
				go.transform.position = new Vector3(go.transform.position.x, -2, go.transform.position.z);
				go.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
			}
		}
	}
}
