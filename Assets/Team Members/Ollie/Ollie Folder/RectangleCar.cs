using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleCar : OllieVehicleBase
{
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        forwardSpeed = 20;
        turnSpeed = 2;
        //grounded = true;
        car = this.gameObject;
    }
}
