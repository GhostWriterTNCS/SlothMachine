using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wormIdle : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.GetComponent<BodyPartTarget>())
        {
            other.GetComponent<BodyPartTarget>().robot.UpdateHealth(-25);
        }
    }
}
