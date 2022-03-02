using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacle : MonoBehaviour
{
    private Rigidbody rb;

    public float maxLength = 20f;
    public float dragMultiplier = 10f;
    public float speedReductionMultiplier = 4f;
    public float minSpeed = -0.2f;
    //public float maxTurnForce = 5f;

    public enum RayDirection
    {
        Left,
        Right,
        Straight
    }

    public RayDirection myTurnDirection;

    MoveForwards moveForwards;
    Neighbours neighbours;
    
    [Header("Reference ONLY:")]
    public float turnForce;
    public float distance;
    Vector3 localVelocity;
    public float xVelocity;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        moveForwards = GetComponentInParent<MoveForwards>();
        neighbours = GetComponentInParent<Neighbours>();
    }

    private void Update()
    {
        //This may be bad because this script needs to know about neighbours now
        if(neighbours.neighbours.Count == 0)
        {
            minSpeed = 0.25f;
        }
        else
        {
            minSpeed = -0.5f;
        }
    }

    void FixedUpdate()
    {
        RaycastHit hitinfo;
        hitinfo = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward, out hitinfo, maxLength, 255, QueryTriggerInteraction.Ignore);

        Debug.DrawRay(transform.position, transform.forward, Color.green);

        //X Velocity is used to make sure we aren't turning against an already existing torque - see below
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        xVelocity = localVelocity.x;

        //Only run this code if we hit a collider
        if (hitinfo.collider)
        {        
            Debug.DrawLine(transform.position, hitinfo.point, Color.green);

            //How far the object is from us
            distance = hitinfo.distance;

            //Adjust speed based on distance to closest collision - clamped between 1 & the max speed
            moveForwards.speed = Mathf.Clamp(moveForwards.speed - (distance / speedReductionMultiplier), minSpeed, moveForwards.maxSpeed);

            //Apply torque based on ray direction (if the left ray hits an object turn right to dodge it)
            if (myTurnDirection == RayDirection.Right)
            {
                //Apply Negative turnforce to turn left
                LeftTurnForce();
            }
            else if (myTurnDirection == RayDirection.Left)
            {
                //Apply Positive turnforce to turn right
                RightTurnForce();
            }
            else if(myTurnDirection == RayDirection.Straight)
            {
                //if we're already moving in a positive rotation (right) - keep moving right
                if(xVelocity < 0)
                {
                    RightTurnForce();
                }
                else
                {
                    LeftTurnForce();
                }
            }


            rb.AddRelativeTorque(0, turnForce, 0, ForceMode.Acceleration);
        }
        else
        {
            //If we aren't detecting any collisions - resume full speed
            moveForwards.speed = moveForwards.maxSpeed;
        }

    }

    void LeftTurnForce()
    {
        turnForce = (maxLength - distance) / -dragMultiplier;
    }

    void RightTurnForce()
    {
        turnForce = (maxLength - distance) / dragMultiplier;
    }
}
