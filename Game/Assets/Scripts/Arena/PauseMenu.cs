using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseMenu : MonoBehaviour
{
	public Robot robot;

    void Update()
    {
        if (Input.GetButtonUp("Menu"))
        {
            Pause();
        }
    }

    public void Pause()
    {
        if(!robot)
        {
            return;
        }
		gameObject.SetActive(!robot.paused);
		robot.paused = !robot.paused;
    }

    public void Leave()
    {
        NetworkManager.singleton.StopClient();
    }
}
