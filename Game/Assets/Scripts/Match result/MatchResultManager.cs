using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultManager : MonoBehaviour {
	public Transform container;
	public Button backToStart;

	void Start() {
		backToStart.gameObject.SetActive(false);
	}
}
