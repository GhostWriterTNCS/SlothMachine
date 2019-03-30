using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitted : MonoBehaviour
{
	public Animator animator;
	List<Collider> siblings = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
		Transform t = transform.parent;
		while(t.parent) {
			t = t.parent;
		}
		siblings.AddRange(t.GetComponentsInChildren<Collider>());
	}

    // Update is called once per frame
    void Update()
    {

	}

	private void OnTriggerEnter(Collider other) {
		if (animator && !siblings.Contains(other) && other.isTrigger) {
			Debug.Log(name + " collided by " + other.name);
			other.enabled = false;
			animator.SetTrigger("Reaction");
			//animator.Play("Reaction", -1, 0);
		}
	}
}
