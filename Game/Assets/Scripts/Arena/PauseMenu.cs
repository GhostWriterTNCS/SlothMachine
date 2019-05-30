using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenu;
	public Robot robot;	
	
    public void Pause()
    {
        if(!robot)
        {
            return;
        }
		pauseMenu.SetActive(!robot.paused);
		robot.paused = !robot.paused;
    }

    public void Leave()
    {
        NetworkManager.singleton.StopClient();
    }
}
