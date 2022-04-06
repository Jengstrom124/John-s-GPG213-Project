using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OlliePlayerAvatar : MonoBehaviour
{
    public bool grounded;
    public Rigidbody rb;
    public float forwardSpeed;
    public float turnSpeed;
    private Vector3 pos;

    public delegate void TouchingCar(GameObject target);
    public event TouchingCar touchingCarEvent;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        forwardSpeed = 10;
        turnSpeed = 3f;
        grounded = true;
    }

    private void Update()
    {
        pos = transform.position;
    }

    public void Forward()
    {
        if (grounded)
        {
            rb.AddRelativeForce(0,0,forwardSpeed);
        }
    }

    public void Backward()
    {
        if (grounded)
        {
            rb.AddRelativeForce(0,0,-forwardSpeed);
        }
    }

    public void Left()
    {
        if (grounded)
        {
            rb.AddRelativeTorque(0, -turnSpeed,0);
        }
    }
    
    public void Right()
    {
        if (grounded)
        {
            rb.AddRelativeTorque(0, turnSpeed,0);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<OllieVehicleBase>() != null)
        {
            touchingCarEvent?.Invoke(other.gameObject);
        }
    }
}
