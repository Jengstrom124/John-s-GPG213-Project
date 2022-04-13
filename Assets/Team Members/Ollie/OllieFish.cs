using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OllieFish : OllieVehicleBase
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        forwardSpeed = 40;
        turnSpeed = 20;
        car = this.gameObject;
    }
}
