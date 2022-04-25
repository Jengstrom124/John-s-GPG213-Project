using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kshark : MonoBehaviour, IControllable, IReactsToWater
{
    
    public Rigidbody sharkRb;
    
    public float sharkSpeed;
    
    public float currentSteeringAngle;
    public float steeringMax = 30f; 
    
    /*public float turningSpeed;
    public float pivotAmount;*/
    public float reduction = 0.5f;
    
    public GameObject sharkObject;

    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;
    public Vector3 tailPosition;

    public Transform tailTransform; 
    
    // Start is called before the first frame update
    void Start()
    {
        sharkRb = sharkObject.GetComponent<Rigidbody>();
        tailPosition = sharkObject.transform.position;
    }

    // Update is called once per frame
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
        /*if (input < 0)
        {
            sharkRb.AddRelativeForce(new Vector3(0f, 0f, pivotAmount));
            sharkRb.AddRelativeTorque(new Vector3(0f, -turningSpeed, 0f));
        }

        if (input > 0)
        {
            sharkRb.AddRelativeForce(new Vector3(0f, 0f, pivotAmount));
            sharkRb.AddRelativeTorque(new Vector3(0f, turningSpeed, 0f));
        }*/
    }

    public void Accelerate(float input)
    {
        //sharkRb.AddRelativeForce(new Vector3(0f, 0f, sharkSpeed*(input*2f)));
        sharkRb.AddForceAtPosition(input*sharkSpeed*transform.TransformDirection(Vector3.forward), transform.position,0);
    }

    public void Reverse(float input)
    {
        //sharkRb.AddRelativeForce(new Vector3(0f, 0f, -sharkSpeed*input));
        sharkRb.AddForceAtPosition(input*sharkSpeed*transform.TransformDirection(Vector3.back), transform.position,0);
    }

    public void Action()
    {
        
    }

    public void Action2()
    {
        
    }

    public void Action3()
    {
       
    }

    public bool IsWet { get; set; }
}
