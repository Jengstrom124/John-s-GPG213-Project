using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacle : MonoBehaviour
{
    private Rigidbody rb;

    public float maxLength = 20f;
    public float dragMultiplier = 10f;
    public float speedReductionMultiplier = 4f;
    //public float maxTurnForce = 5f;

    public enum RayDirection
    {
        Left,
        Right,
        Straight
    }

    public RayDirection myTurnDirection;

    MoveForwards moveForwards;
    
    [Header("Reference ONLY:")]
    public float turnForce;
    public float distance;
    Vector3 localVelocity;
    public float xVelocity;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        moveForwards = GetComponentInParent<MoveForwards>();
    }

    void FixedUpdate()
    {
        RaycastHit hitinfo;
        hitinfo = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward, out hitinfo, maxLength);

        localVelocity = transform.InverseTransformDirection(rb.velocity);
        xVelocity = localVelocity.x;

        //Only run this code if we hit a collider
        if (hitinfo.collider)
        {        
            Debug.DrawLine(transform.position, hitinfo.point, Color.green);

            distance = hitinfo.distance;

            moveForwards.speed = Mathf.Clamp(moveForwards.speed - (distance / speedReductionMultiplier), 1f, moveForwards.maxSpeed);

            if (myTurnDirection == RayDirection.Right)
            {
                turnForce = (maxLength - distance) / -dragMultiplier;
            }
            else if (myTurnDirection == RayDirection.Left)
            {
                turnForce = (maxLength - distance) / dragMultiplier;
            }
            else if(myTurnDirection == RayDirection.Straight)
            {
                if(xVelocity < 0)
                {
                    turnForce = (maxLength - distance) / dragMultiplier;
                }
                else
                {
                    turnForce = (maxLength - distance) / -dragMultiplier;
                }
            }


            rb.AddRelativeTorque(0, turnForce, 0, ForceMode.Acceleration);
        }
        else
        {
            moveForwards.speed = moveForwards.maxSpeed;
        }

    }
}
