using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class OllieVehicleBase : NetworkBehaviour, IControllable, IReactsToWater, IStateBase, IPredator, IReactsToInk
{
    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    public float forwardSpeed;
    public float groundSpeed;
    public float turnSpeed;
    public float wiggleSpeed;
    public Vector3 localVelocity;
    public bool playerInVehicle;
    public Transform tailTurnPoint;
    public GameObject car;
    public bool boosting = false;
    public bool jumping;
    public float jumpHeight;
    public float jumpDistance;
    public Transform model;
    public RigidbodyConstraints originalConstraints;
    public float originalDrag, originalAngularDrag;
    public IPredator iPredator;
    public Transform bumPoint;
    public Stomach stomach;

    //left overs ported from Vehicles project - maybe needed later for changing sharks
    //although I think this is unlikely
    // public delegate void ExitVehicle();
    // public static event ExitVehicle exitVehicleEvent;
    

    private void Update()
    {
        //both are currently unused
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        wiggleSpeed = UnityEngine.Random.Range(-1f, 1f);
    }


    
    public virtual void WaterCheck()
    {
        
    }

    #region IControllable Interface

    public void Steer(float amount)
    {
        if (IsServer)
        {
            if (amount > 0)
            {
                rb.AddRelativeTorque(0, turnSpeed,0,ForceMode.Acceleration);
            }
           
            else if (amount < 0)
            {
                rb.AddRelativeTorque(0, -turnSpeed,0,ForceMode.Acceleration);
            }
            else
            {
                // put wiggle code here if I get it working
            }
        }
    }

    public void Accelerate(float amount)
    {
        if (IsServer)
        {
            if (amount > 0)
            {
                rb.AddRelativeForce(0, 0, forwardSpeed);
            }
        }
    }

    public void Reverse(float amount)
    {
        if (IsServer)
        {
            if (amount > 0)
            {
                //print("sharks can't reverse, dummy");
            }
        }
    }

    public virtual void Action(InputActionPhase aActionPhase) // F key
    {
        
    }
    public virtual void Action2(InputActionPhase aActionPhase) // E key
    {
        
    }
    public virtual void Action3(InputActionPhase aActionPhase) // Q key
    {
        
    }
    #endregion

    #region IStateBase Interface - I don't know if I'm using this yet
    public void Enter()
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
    #endregion
    
    #region Hardcoded Movement - Obsolete
    // public void Forward()
    // {
    //     rb.AddRelativeForce(0,0,forwardSpeed);
    //     print("going forward");
    // }

    // public void Backward()
    // {
    //     if(backward)
    //         if (grounded && playerInVehicle)
    //         {
    //             rb.AddRelativeForce(Vector3.back * forwardSpeed);
    //         }
    // }

    /*public void Left()
    {
        // if (Input.GetKey(KeyCode.A))
        // {
        //     left = true;
        // }
        // else
        // {
        //     left = false;
        // }
    }
    
    public void Right()
    {
        // if (Input.GetKey(KeyCode.D))
        // {
        //     right = true;
        // }
        // else
        // {
        //     right = false;
        // }
    }*/

    /*public void Steering()
    {
        if (left)
        {
            rb.AddRelativeTorque(0, -turnSpeed,0,ForceMode.Acceleration);
        }
        else if (right)
        {
            rb.AddRelativeTorque(0, turnSpeed,0,ForceMode.Acceleration);
        }
        else
        {
            //rb.AddForceAtPosition((new Vector3(wiggleSpeed,0,0)),tailTurnPoint.position,ForceMode.Acceleration);
        }
    }*/
    #endregion

    #region IReactsToWater Interface

    public bool IsWet { get; set; }

    #endregion

    #region IPredator Interface
    public Vector3 GetBumPosition()
    {
        return bumPoint.transform.position;
    }
    #endregion

    #region IReactsToInk
    public void ChangeInkedState(bool isInked)
    {
        throw new NotImplementedException();
    }
    #endregion
}
