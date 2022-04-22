using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kevin
{
    public class Seal : MonoBehaviour, IControllable, IReactsToWater
{
    //Gameobject Variables
    public Rigidbody sealRigidbody;
    public GameObject sealPrefab;
    
    //Vectors
    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;
    public Vector3 tailPosition;
    
    //Transforms
    public Transform headTransform; 
    public Transform tailTransform;
    
    
    //Wiggle Transforms
    public Transform tailLeadTransform;
    public Transform tailMidTransform;
    public Transform tailTipTransform;
    
    //flipper Transforms
    public Transform leftFlipper;
    public Transform rightFlipper;
    
    //Stats/Floats Variables
    public float accelerationSpeed;
    public float currentSteeringAngle;
    public float steeringMax = 30f;
    public float reduction = 0.5f;

    public float currentBobbleAngle;
    
    //Flippers Animation
    public float maxAngle;
    public float currentFlipperAngle;
    
    
    // Start is called before the first frame update
    void Start()
    {
       
        sealRigidbody = sealPrefab.GetComponent<Rigidbody>();
        tailPosition = sealPrefab.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tailPosition = tailTransform.position;
        localVelocity = transform.InverseTransformDirection(sealRigidbody.velocity);
        tailLocalVelocity = tailTransform.InverseTransformDirection(sealRigidbody.GetPointVelocity(tailPosition));
        
        sealRigidbody.AddRelativeForce(sealRigidbody.mass*new Vector3 (-localVelocity.x, 0, 0));
        
        sealRigidbody.AddForceAtPosition(sealRigidbody.mass*tailTransform.
            TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailPosition);
        
        localVelocity = transform.InverseTransformDirection(sealRigidbody.velocity);
        
        sealRigidbody.AddRelativeForce(new Vector3(-localVelocity.x,0f,0f));

        if (localVelocity.z > 0) 
        {
            StartCoroutine(Decelerate());
        }
    }

  

    IEnumerator Decelerate()
    {
        sealRigidbody.AddRelativeForce(new Vector3(0f,0f,-1f));
        yield return new WaitForSeconds(reduction);
    }

    public void Steer(float input)
    {
        float currentYEuler = transform.eulerAngles.y;
        float targetAngle = 0;

        if (input == 0f)
        {
            //targetAngle = 
        }
        else
        {
            targetAngle = -input * steeringMax;
        }
    
        

        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, 0.1f);
        
        tailTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
        tailLeadTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.25f * currentSteeringAngle ,0f);
        tailMidTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.5f * currentSteeringAngle ,0f);
        tailTipTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.75f * currentSteeringAngle ,0f);
        //headTransform.eulerAngles = new Vector3(0f,0f , currentYEuler + 0.75f * currentSteeringAngle);
    }

    public void Accelerate(float input)
    {
        
        /*Vector3 currentHeadPosition = headTransform.transform.position;
        if (input > 0.5)
        {
            sealRigidbody.AddForceAtPosition(input*accelerationSpeed*transform.TransformDirection(Vector3.forward), transform.position,0);
            headTransform.eulerAngles = new Vector3(60f, 0f, 0f);
            //headTransform.transform.position = new Vector3(headTransform.position.x, headTransform.position.y - 0.075f, headTransform.position.z + 0.5f);
        }
        else
        {
            headTransform.transform.position = currentHeadPosition;
        }*/
        
        /*float currentYEuler = transform.eulerAngles.y;
        float targetAngle = 0;
        
        if(input > 0.5f)
        {
            targetAngle = -input * maxAngle;
        }
        
        currentFlipperAngle = Mathf.Lerp(currentFlipperAngle, targetAngle, 0.01f);

        leftFlipper.eulerAngles = new Vector3(0f,0f ,currentYEuler * currentFlipperAngle);
        rightFlipper.eulerAngles = new Vector3(0f, 0f, currentYEuler * currentFlipperAngle);*/

        float currentYEuler = transform.eulerAngles.y;
        float currentXEuler = transform.eulerAngles.x;
        float targetAngle = 0;
        
        if (input > 0.5f)
        {
            targetAngle = input * maxAngle;
        }
        else
        {
            targetAngle = input;
            //headTransform.eulerAngles = new Vector3(80f,0f, 0f);
        }

        currentBobbleAngle = Mathf.Lerp(currentBobbleAngle, targetAngle, 0.5f);
        
        headTransform.eulerAngles = new Vector3(currentXEuler + currentBobbleAngle,currentYEuler, 0f);
        
        //headTransform.transform.position = new Vector3(headTransform.position.x, headTransform.position.y - 0.075f, headTransform.position.z + 0.5f);
        sealRigidbody.AddForceAtPosition(input*accelerationSpeed*transform.TransformDirection(Vector3.forward), transform.position,0);

    }

    public void Reverse(float input)
    {
        sealRigidbody.AddForceAtPosition(input*accelerationSpeed*transform.TransformDirection(Vector3.back), transform.position,0);
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
}

