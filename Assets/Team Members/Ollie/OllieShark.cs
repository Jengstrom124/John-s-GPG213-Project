using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OllieShark : OllieVehicleBase
{
    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        groundSpeed = 22.5f;
        forwardSpeed = 35;
        turnSpeed = 10;
        car = this.gameObject;
    }
}
