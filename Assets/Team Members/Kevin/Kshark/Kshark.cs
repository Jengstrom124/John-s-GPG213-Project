using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kshark : MonoBehaviour, IControllable
{
    
    public Rigidbody sharkRb;
    
    public float sharkSpeed;
    public float turningSpeed;
    public float pivotAmount;
    public float reduction = 0.5f;
    
    public GameObject sharkObject;

    public Vector3 localVelocity;
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
        
        localVelocity = transform.InverseTransformDirection(sharkRb.velocity);
        
        sharkRb.AddRelativeForce(new Vector3(-localVelocity.x,0f,0f));

        if (localVelocity.z > 0) 
        {
            StartCoroutine(Decelerate());
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

    IEnumerator Decelerate()
    {
        sharkRb.AddRelativeForce(new Vector3(0f,0f,-1f));
        yield return new WaitForSeconds(reduction);
    }

    public void Steer(float input)
    {
        if (input < 0)
        {
            sharkRb.AddRelativeForce(new Vector3(0f, 0f, pivotAmount));
            sharkRb.AddRelativeTorque(new Vector3(0f, -turningSpeed, 0f));
        }

        if (input > 0)
        {
            sharkRb.AddRelativeForce(new Vector3(0f, 0f, pivotAmount));
            sharkRb.AddRelativeTorque(new Vector3(0f, turningSpeed, 0f));
        }
    }

    public void Accelerate(float input)
    {
        sharkRb.AddRelativeForce(new Vector3(0f, 0f, sharkSpeed*(input*2f)));
    }

    public void Reverse(float input)
    {
        sharkRb.AddRelativeForce(new Vector3(0f, 0f, -sharkSpeed*input));
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
