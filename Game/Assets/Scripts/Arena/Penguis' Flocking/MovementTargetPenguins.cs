using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTargetPenguins : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0f, 30f)] public float range = 5f;
    [Range(0f, 30f)] public float moveSpeed = 5f;
    private Vector3 destination;
    private Rigidbody myRigidbody;


    private void Start()
    {
        destination = new Vector3(20,0,20);
        myRigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (destination!= myRigidbody.position)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            //myRigidbody.MovePosition(myRigidbody.position + transform.forward * moveSpeed * Time.deltaTime);
        }
        
    }
}
