using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinsSpawner : MonoBehaviour
{
    public float radius = 10f;
    public int count = 30;
    public GameObject penguin = null;

    void Start()
    {
        if (penguin != null)
        {
            for (int i = 0; i < count; i += 1)
            {
                GameObject go = Instantiate(penguin, transform.position + Random.insideUnitSphere * radius, Quaternion.EulerAngles(0,Random.RandomRange(0,359),0));
                go.transform.LookAt(transform.position + Random.insideUnitSphere * radius);
                go.GetComponentInChildren<MeshRenderer>().transform.Rotate(90,0,0);
                go.transform.position= new Vector3(go.transform.position.x, 0.8f, go.transform.position.z);
                go.name = penguin.name + " " + i;
               
            }
        }
    }
}
