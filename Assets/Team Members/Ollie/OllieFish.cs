using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OllieFish : OllieVehicleBase
{
    public IStateBase flock;
    public IStateBase pathFollow;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        forwardSpeed = 40;
        turnSpeed = 20;
        car = this.gameObject;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            flock.Enter();
        }

        if (Input.GetKey(KeyCode.B))
        {
            flock.Exit();
        }
    }

}
