using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kshark : MonoBehaviour
{
    
    public Rigidbody sharkRb;
    public float sharkSpeed;

    public float reduction = 0.5f;
    
    public GameObject sharkObject;

    public Vector3 localVelocity;
    public Vector3 tailPosition;
    
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
            sharkRb.AddRelativeForce(new Vector3(0f, 0f, sharkSpeed*2));
        }
        if (Input.GetKey(KeyCode.S))
        {
            sharkRb.AddRelativeForce(new Vector3(0f, 0f, -sharkSpeed));
        }
        if (Input.GetKey(KeyCode.A))
        {
            sharkRb.AddRelativeTorque(new Vector3(0f, -4f, 0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            sharkRb.AddRelativeTorque(new Vector3(0f, 4f, 0f));
        }
    }

    IEnumerator Decelerate()
    {
        sharkRb.AddRelativeForce(new Vector3(0f,0f,-1f));
        yield return new WaitForSeconds(reduction);
    }
}
