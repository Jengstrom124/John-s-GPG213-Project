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
        IReactsToWater boolInterface = other.GetComponent<IReactsToWater>();
        if (boolInterface != null)
        {
            boolInterface.IsWet = true;
            other.GetComponent<KFish>().iWet = true;
            Debug.Log("Splash!!!");
        }
        
    }

    void OnTriggerExit (Collider other)
    {
        IReactsToWater boolInterface = other.GetComponent<IReactsToWater>();
        if (boolInterface != null)
        {
            boolInterface.IsWet = false;
            other.GetComponent<KFish>().iWet = false;
            Debug.Log("Splosh!!!");
        }
    }
    
}
