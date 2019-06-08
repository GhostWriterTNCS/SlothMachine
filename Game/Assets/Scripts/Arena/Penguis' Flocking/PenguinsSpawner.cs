using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinsSpawner : MonoBehaviour
{
    public float radius = 10f;
    public int count = 5;
    public GameObject penguin = null;

    void Start()
    {
        if (penguin != null)
        {
            for (int i = 0; i < count; i += 1)
            {
                GameObject go = Instantiate(penguin, transform.position + Random.insideUnitSphere * radius, transform.rotation);
                go.transform.LookAt(transform.position + Random.insideUnitSphere * radius);
                go.name = penguin.name + " " + i;
            }
        }
    }
}
