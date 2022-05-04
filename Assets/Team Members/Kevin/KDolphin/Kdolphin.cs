using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class Kdolphin : MonoBehaviour, IControllable, IReactsToWater, IRTS
{
    
    public Rigidbody sharkRb;
    
    public float sharkSpeed;
    public float jumpPower;
    public float divePower;
    public float currentSteeringAngle;
    public float steeringMax = 30f; 
    
    /*public float turningSpeed;
    public float pivotAmount;*/
    public float reduction = 0.5f;
    
    public GameObject sharkObject;

    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;
    public Vector3 tailPosition;

    public Transform headTipTransform; 
    public Transform tailTransform;

    public bool canLeap;
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

        if (IsWet == true|| transform.position.y < 8f)
        {
            sharkRb.useGravity = false;
            canLeap = true;
            sharkRb.AddForceAtPosition(transform.TransformDirection(Vector3.up)/2f, headTipTransform.position, 0);
        }
        else
        {
            sharkRb.useGravity = true;
            canLeap = false;
            sharkRb.AddForceAtPosition(transform.TransformDirection(Vector3.down)/2f, headTipTransform.position, 0);
        }
    }

    IEnumerator Decelerate()
    {
        sharkRb.AddRelativeForce(new Vector3(0f,0f,-1f));
        yield return new WaitForSeconds(reduction);
    }

    IEnumerator Dive()
    {
        sharkRb.AddForceAtPosition(divePower*transform.TransformDirection(Vector3.down),headTipTransform.position,0f);
        yield return new WaitForSeconds(0.5f);
    }

    public void Steer(float input)
    {
        float currentYEuler = transform.eulerAngles.y;
        float targetAngle = 0;
        
        targetAngle = -input * steeringMax;
        

        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, 0.1f);
        tailTransform.eulerAngles = new Vector3(0f, (currentYEuler + 2f * currentSteeringAngle), 0f);
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

    public void Action(InputActionPhase aActionPhase)
    {
        if (canLeap == true)
        {
            sharkRb.AddForceAtPosition(jumpPower*transform.TransformDirection(Vector3.up), headTipTransform.position, 0);
            sharkRb.AddForceAtPosition(jumpPower/2f*transform.TransformDirection(Vector3.forward), transform.position, 0);
            StartCoroutine(Dive());
        }
   
    }

    public void Action2(InputActionPhase aActionPhase)
    {
        
    }

    public void Action3(InputActionPhase aActionPhase)
    {
       
    }

    public bool IsWet { get; set; }
    public void Selected()
    {
        throw new System.NotImplementedException();
    }

    public void Deselected()
    {
        throw new System.NotImplementedException();
    }

    public void SetDestination(Vector3 position)
    {
        throw new System.NotImplementedException();
    }
}
