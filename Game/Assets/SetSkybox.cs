using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkybox : MonoBehaviour {
	public Material skybox;

	void Start() {
		RenderSettings.skybox = skybox;
	}
}
