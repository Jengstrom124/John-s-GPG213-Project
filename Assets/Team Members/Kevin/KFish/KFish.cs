using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Luke;
using Sirenix.OdinInspector;
using UnityEngine;
using Flock = Kevin.Flock;
using Vector3 = UnityEngine.Vector3;

public class KFish : SerializedMonoBehaviour, IControllable, IReactsToWater, IStateBase
{
    //State Base
    public IStateBase flock;
    public IStateBase pathFollow;

    //Bool Checker
    public bool iWet;
    
    //Fish Movement Variables
    public Rigidbody fRb;
    public Transform tailObjectTransform;

    public float acceleration = 10f;
    public float currentSteeringAngle;
    public float steeringMax = 45f;
    
    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;

    void Start()
    {
        fRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 tailPosition = tailObjectTransform.position;
        localVelocity = transform.InverseTransformDirection(fRb.velocity);
        tailLocalVelocity = tailObjectTransform.InverseTransformDirection(fRb.GetPointVelocity(tailPosition));
        
        
        fRb.AddForceAtPosition(fRb.mass*tailObjectTransform.
            TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailPosition);
        
        if (localVelocity.z > 0) 
        {
            //StartCoroutine(Decelerate());
        }
        if (Input.GetKey(KeyCode.W))
        {
            Accelerate(1f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Reverse(1f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Steer(-1f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Steer(1f);
        }
    }
    public bool IsWet { get; set; }
    public void Steer(float input)
    {
        float currentYEuler = transform.eulerAngles.y;
        float targetAngle = 0;
        
        if (input < 0f)
        {
            targetAngle = -input * steeringMax;
        }

        if (input > 0f)
        {
            targetAngle = -input * steeringMax;
        }

        if (input == 0f)
        {
            targetAngle = -input;
        }

        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, 0.1f);
        tailObjectTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
    }

    public void Accelerate(float input)
    {
        fRb.AddForceAtPosition(input*acceleration*transform.TransformDirection(Vector3.down), transform.position,0);
    }

    public void Reverse(float input)
    {
        throw new System.NotImplementedException();
    }

    public void Action()
    {
        throw new System.NotImplementedException();
    }

    public void Action2()
    {
        throw new System.NotImplementedException();
    }

    public void Action3()
    {
        throw new System.NotImplementedException();
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

   
}
