using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public float HoverForce;
    public Rigidbody physicsRef;
    public float RotationDrag;
    public float MaxHover;
    public float Speed;
    private Vector3 direction;
    private Vector3 localVelocity;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        direction = transform.TransformDirection(Vector3.down);
        
        if (Physics.Raycast(transform.position, direction, out hit,1f))
        {
            float force = 1 - hit.distance;
            force *= HoverForce;
            if (hit.distance >= MaxHover)
            {
                force = force / 3;
            }
            physicsRef.AddForceAtPosition(new Vector3(0,force,0),transform.position);
            Debug.DrawLine(transform.position,hit.point,Color.magenta);
        }
        physicsRef.AddForceAtPosition(new Vector3(0,0,Input.GetAxis("Vertical") * Speed),transform.position);
        
        localVelocity = transform.InverseTransformDirection(physicsRef.velocity);
        physicsRef.AddRelativeForce(-localVelocity.x * RotationDrag,0,0);
    }
}
