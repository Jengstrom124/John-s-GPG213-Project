using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    public void Update()
    {
        
    }

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
