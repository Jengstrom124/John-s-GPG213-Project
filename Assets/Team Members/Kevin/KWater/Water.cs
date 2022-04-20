using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : ManagerBase<Water>
{
    void OnTriggerEnter(Collider other)
    {
        IReactsToWater reactsToWater = other.GetComponent<IReactsToWater>();
        if (reactsToWater != null)
        {
            reactsToWater.IsWet = true;
        }
        
    }

    void OnTriggerExit (Collider other)
    {
        IReactsToWater reactsToWater = other.GetComponent<IReactsToWater>();
        if (reactsToWater != null)
        {
            reactsToWater.IsWet = false;
        }
    }
    
}
