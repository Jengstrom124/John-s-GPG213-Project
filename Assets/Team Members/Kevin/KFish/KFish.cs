using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KFish : MonoBehaviour, IControllable
{
    public Transform tailSteeringTransform;
    
    public GameObject fishPrefab;

    public Rigidbody fRb; 
    
    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;

    public float fishSpeed;
    // Start is called before the first frame update
    void Start()
    {
        fRb = fishPrefab.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tailPos = tailSteeringTransform.position;
        localVelocity = transform.InverseTransformDirection(fRb.velocity);
        tailLocalVelocity = tailSteeringTransform.InverseTransformDirection((fRb.GetPointVelocity(tailPos)));
        
        //fRb.AddRelativeForce(new Vector3(-localVelocity.x,0f,0f));

        fRb.AddForceAtPosition(tailSteeringTransform.TransformDirection(new Vector3(-tailLocalVelocity.x,0f,0f)), tailPos);
        
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

    public void Steer(float input)
    {
        if (input > 0f)
        {
            tailSteeringTransform.eulerAngles = new Vector3(0f, 90f, 0f);
        }

        if (input < 0f)
        {
            tailSteeringTransform.eulerAngles = new Vector3(0f, -90f, 0f);
        }
    }

    public void Accelerate(float input)
    {
        fRb.AddRelativeForce(new Vector3(0f, -fishSpeed*(input*2f)), 0f);
    }

    public void Reverse(float input)
    {
        fRb.AddRelativeForce(new Vector3(0f, fishSpeed*input), 0f);
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
}
