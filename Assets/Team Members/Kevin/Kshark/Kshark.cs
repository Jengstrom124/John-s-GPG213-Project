using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kevin
{
    public class Kshark : MonoBehaviour, IControllable, IReactsToWater
{
    
    public Rigidbody sharkRb;
    
    public float sharkSpeed;
    public float currentSteeringAngle;
    public float steeringMax = 30f;
    public float reduction = 0.5f;
    
    public GameObject sharkObject;

    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;
    public Vector3 tailPosition;

    public Transform headTransform;
    public Transform tailTransform;
    public Transform tail1Transform;
    public Transform tail2Transform;
    public Transform tail3Transform;
    public Transform tail4Transform;
    void Start()
    {
        sharkRb = sharkObject.GetComponent<Rigidbody>();
        tailPosition = sharkObject.transform.position;
    }
    
    void Update()
    {
        Vector3 tailPosition = tailTransform.position;
        localVelocity = transform.InverseTransformDirection(sharkRb.velocity);
        tailLocalVelocity = tailTransform.InverseTransformDirection(sharkRb.GetPointVelocity(tailPosition));
        
        sharkRb.AddRelativeForce(sharkRb.mass*new Vector3 (-localVelocity.x, 0, 0));
        sharkRb.AddForceAtPosition(sharkRb.mass*tailTransform.
            TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailPosition);
        localVelocity = transform.InverseTransformDirection(sharkRb.velocity);
        sharkRb.AddRelativeForce(new Vector3(-localVelocity.x,0f,0f));

        if (localVelocity.z > 0) 
        {
            StartCoroutine(Decelerate());
        }
    }

    IEnumerator Decelerate()
    {
        sharkRb.AddRelativeForce(new Vector3(0f,0f,-1f));
        yield return new WaitForSeconds(reduction);
    }

    public void Steer(float input)
    {
        float currentYEuler = transform.eulerAngles.y;
        float targetAngle = 0;
        
        targetAngle = -input * steeringMax;
        
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, 0.1f);
        
        tailTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
        
        tail1Transform.eulerAngles = new Vector3(90, currentYEuler+0.5f*currentSteeringAngle, 180);
        tail3Transform.eulerAngles = new Vector3(-90, currentYEuler+1.5f*currentSteeringAngle, 0);
        tail4Transform.eulerAngles = new Vector3(-90, currentYEuler+2f*currentSteeringAngle, 0);
        
        headTransform.eulerAngles = new Vector3(90, 0, -(currentYEuler-0.5f*currentSteeringAngle));
        
    }

    public void Accelerate(float input)
    {
        sharkRb.AddForceAtPosition(input*sharkSpeed*transform.TransformDirection(Vector3.forward), transform.position,0);
    }

    public void Reverse(float input)
    {
        sharkRb.AddForceAtPosition(input*sharkSpeed*transform.TransformDirection(Vector3.back), transform.position,0);
    }

    public void Action(InputActionPhase aActionPhase)
    {
        
    }

    public void Action2(InputActionPhase aActionPhase)
    {
        
    }

    public void Action3(InputActionPhase aActionPhase)
    {
       
    }

    public bool IsWet { get; set; }
}
}

