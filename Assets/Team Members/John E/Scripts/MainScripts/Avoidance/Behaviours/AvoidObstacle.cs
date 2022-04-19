using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacle : MonoBehaviour
{
    private Rigidbody rb;

    [Header("General Configurations:")]
    [Tooltip("Maximum Feeler Reach")]
    public float maxLength = 20f;
    [Tooltip("Higher drag = more friction = lower turn sensitivty")]
    public float dragMultiplier = 10f;

    [Header("Extra Configurations (Not Required/Ignore)")]
    [Tooltip("Specify what to look for / ignore")]
    public LayerMask layerMask;
    public RayDirection myTurnDirection;
    public bool isEmergencyFeeler = false;
    public float speedReductionMultiplier = 4f;
    public float minSpeed = 0f;

    public enum RayDirection
    {
        Left,
        Right,
        Straight
    }

    [Header("Debug Options: ")]
    public bool visualizeRays = false;

    MoveForwards moveForwards;
    
    [Header("Reference ONLY:")]
    public float turnForce;
    public float distance;
    Vector3 localVelocity;
    public float xVelocity;
    public bool updateSpeed = false;
    public bool feelerActive = false;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        moveForwards = GetComponentInParent<MoveForwards>();
    }

    void FixedUpdate()
    {
        RaycastHit hitinfo;
        hitinfo = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward, out hitinfo, maxLength, layerMask.value, QueryTriggerInteraction.Ignore);

        //X Velocity is used to make sure we aren't turning against an already existing torque - see below
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        xVelocity = localVelocity.x;

        //Only run this code if we hit a collider
        if (hitinfo.collider)
        {
            feelerActive = true;

            //Visualizing RayCasts
            if(visualizeRays)
            {
                if (isEmergencyFeeler)
                {
                    Debug.DrawLine(transform.position, hitinfo.point, Color.red);
                }
                else
                {
                    Debug.DrawLine(transform.position, hitinfo.point, Color.green);
                }
            }

            //How far the object is from us
            distance = hitinfo.distance;

            //Adjust speed based on distance to closest collision - clamped between 1 & the max speed
            if (moveForwards != null)
            {
                if(isEmergencyFeeler)
                {
                    moveForwards.speed = Mathf.Clamp(moveForwards.speed = distance - speedReductionMultiplier, minSpeed, moveForwards.maxSpeed);
                }
                else
                {
                    moveForwards.speed = Mathf.Clamp(moveForwards.speed = distance/3, minSpeed, moveForwards.maxSpeed);
                }
            }
            //Apply torque based on ray direction (if the left ray hits an object turn right to dodge it)
            if (myTurnDirection == RayDirection.Right)
            {
                //Apply Negative turnforce to turn left
                TurnForce(-dragMultiplier);
            }
            else if (myTurnDirection == RayDirection.Left)
            {
                //Apply Positive turnforce to turn right
                TurnForce(dragMultiplier);
            }
            else if (myTurnDirection == RayDirection.Straight)
            {
                //if we're already moving in a positive rotation (right) - keep moving right
                if (xVelocity < 0)
                {
                    TurnForce(dragMultiplier/2);
                }
                else
                {
                    TurnForce(-dragMultiplier/2);
                }
            }

            rb.AddRelativeTorque(0, turnForce, 0, ForceMode.Acceleration);
        }
        else
        {
            //If we aren't detecting any collisions - resume full speed
            //moveForwards.speed = Mathf.Lerp(moveForwards.speed, moveForwards.maxSpeed, 2f);

            feelerActive = false;
        }
    }

    void TurnForce(float multiplier)
    {
        turnForce = (maxLength - distance) / multiplier;
    }
}
