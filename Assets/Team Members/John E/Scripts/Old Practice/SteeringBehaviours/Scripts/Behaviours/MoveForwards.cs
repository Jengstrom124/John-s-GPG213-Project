using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwards : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f;
    public float maxSpeed = 15f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward * speed);
    }
}
