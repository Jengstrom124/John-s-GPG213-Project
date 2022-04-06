using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class OllieVehicleBase : MonoBehaviour
{
    public bool grounded;
    public Rigidbody rb;
    public float forwardSpeed;
    public float turnSpeed;
    public Vector3 localVelocity;
    public bool playerInVehicle;
    public Transform exitPoint;
    public GameObject car;

    private bool forward, backward, left, right;

    public delegate void ExitVehicle();

    public static event ExitVehicle exitVehicleEvent;

    private void Start()
    {
        forward = false;
        backward = false;
        left = false;
        right = false;
    }

    private void Update()
    {
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        if (rb.velocity == (Vector3.zero))
        {
            exitVehicleEvent?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        Forward();
        Backward();
        Left();
        Right();
    }

    public void Forward()
    {
        if(forward)
            if (grounded && playerInVehicle)
            {
                rb.AddRelativeForce(Vector3.forward * forwardSpeed);
            }
    }

    public void Backward()
    {
        if(backward)
            if (grounded && playerInVehicle)
            {
                rb.AddRelativeForce(Vector3.back * forwardSpeed);
            }
    }

    public void Left()
    {
        if(left)
            if (grounded && playerInVehicle)
            {
                rb.AddRelativeForce(-localVelocity/5);
                rb.AddRelativeTorque(0, -turnSpeed,0);
            }
    }
    
    public void Right()
    {
        if(right)
            if (grounded && playerInVehicle)
            {
                rb.AddRelativeForce(-localVelocity/5);
                rb.AddRelativeTorque(0, turnSpeed,0);
            }
    }

    public void Enter()
    {
        playerInVehicle = true;
        car.SetActive(true);
    }

    public void Exit()
    {
        playerInVehicle = false;
    }

    public void Steer(float amount)
    {
        if (amount > 0)
        {
            right = true;
            left = false;
        }
           
        else if (amount < 0)
        {
            right = false;
            left = true;
        }
        else
        {
            right = false;
            left = false;
        }
    }

    public void Accelerate(float amount)
    {
        print("accelerating " + amount);
        if (amount > 0)
        {
            forward = true;
            backward = false;
        }
        else if (amount < 0)
        {
            forward = false;
            backward = true;
        }
        else
        {
            forward = false;
            backward = false;
        }
            
    }

    public Transform GetVehicleExitPoint()
    {
        return exitPoint;
    }

    public bool canEnter()
    {
        if(playerInVehicle)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
