using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacle : MonoBehaviour
{
    private Rigidbody rb;

    public float maxLength = 1f;
    public float maxTurnForce = 5f;
    
    [Header("Reference ONLY:")]
    public float turnForce;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Container for useful info coming from casting functions (note ‘out’ below)
        RaycastHit hitinfo;
        hitinfo = new RaycastHit();
        //Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(transform.position + new Vector3(0,0,5f), transform.forward, out hitinfo, maxLength);
        Debug.DrawLine(transform.position, hitinfo.point, Color.green);

        //Only run this code if we hit a collider
        if (hitinfo.collider)
        {
            float distance = hitinfo.distance;
            turnForce = maxTurnForce - distance;
            
            rb.AddRelativeTorque(0, turnForce, 0);
        }
    }
}
