using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinsBlending : MonoBehaviour
{
    private Collider[] neighbors = new Collider[200];
    // this must be large enough

    private Vector3 destination;
    private Transform pointWhereDie;


    void FixedUpdate()
    {
        destination = GameObject.Find("Penguins Destination").transform.position;
        pointWhereDie = GameObject.Find("Penguins Destination").transform;
        Vector3 globalDirection = destination;

        int count = Physics.OverlapSphereNonAlloc(transform.position, PenguinsShared.PenguinFOW, neighbors);

        foreach (PenguinsComponent bc in GetComponents<PenguinsComponent>())
        {
            globalDirection += bc.GetDirection(neighbors, count);
        }

        if (globalDirection != destination)
        {
            //transform.rotation = Quaternion.LookRotation((globalDirection.normalized + transform.forward) / 2f);
            transform.LookAt(pointWhereDie);
        }

        transform.position += transform.forward * PenguinsShared.PenguinSpeed * Time.deltaTime;
    }
}
