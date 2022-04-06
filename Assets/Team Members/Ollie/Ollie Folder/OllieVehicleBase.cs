using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class OllieVehicleBase : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardSpeed;
    public float turnSpeed;
    public float wiggleSpeed;
    public Vector3 localVelocity;
    public bool playerInVehicle;
    public Transform tailTurnPoint;
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

        wiggleSpeed = UnityEngine.Random.Range(-1f, 1f);
    }

    private void FixedUpdate()
    {
        Forward();
        //Backward();
        Left();
        Right();
        Steering();
    }

    public void Forward()
    {
        rb.AddRelativeForce(0,0,forwardSpeed);
    }

    // public void Backward()
    // {
    //     if(backward)
    //         if (grounded && playerInVehicle)
    //         {
    //             rb.AddRelativeForce(Vector3.back * forwardSpeed);
    //         }
    // }

    public void Left()
    {
        // if(left)
        // {
        //     rb.AddRelativeForce(-localVelocity/5);
        //     rb.AddRelativeTorque(0, -turnSpeed,0);
        // }

        if (Input.GetKey(KeyCode.A))
        {
            left = true;
        }
        else
        {
            left = false;
        }
    }
    
    public void Right()
    {
        // if(right)
        // {
        //     rb.AddRelativeForce(-localVelocity/5);
        //     rb.AddRelativeTorque(0, turnSpeed,0);
        // }
        
        if (Input.GetKey(KeyCode.D))
        {
            right = true;
        }
        else
        {
            right = false;
        }
    }

    public void Steering()
    {
        if (left)
        {
            print("moving left" + left);
            //rb.AddForceAtPosition((new Vector3(turnSpeed,0,-localVelocity.z)),tailTurnPoint.position);
            
            //rb.AddForceAtPosition(Vector3.right,tailTurnPoint.position);
            rb.AddRelativeTorque(0, -turnSpeed,0);
        }
        else if (right)
        {
            print("moving right" + right);
            //rb.AddForceAtPosition((new Vector3(-turnSpeed,0,-localVelocity.z)),tailTurnPoint.position);
            
            //rb.AddForceAtPosition(Vector3.left,tailTurnPoint.position);
            rb.AddRelativeTorque(0, turnSpeed,0);
        }
        else
        {
            rb.AddForceAtPosition((new Vector3(wiggleSpeed,0,0)),tailTurnPoint.position);
        }
    }

    #region IDrivable Interface
    
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

    // public Transform GetVehicleExitPoint()
    // {
    //     return exitPoint;
    // }

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
    #endregion
}
