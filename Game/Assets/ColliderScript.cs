using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColliderScript : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<wormIdle>())
        {
            NetworkManager.Destroy(other.gameObject);
        }
    }
}
