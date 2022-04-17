using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject waterPrefab;
    public Collider waterCollision; 
    
    public void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        IReactsToWater boolInterface = other.GetComponent<IReactsToWater>();
        if (boolInterface != null)
        {
            other.GetComponent<IReactsToWater>().IsWet = true; 
            Debug.Log("Splash!!!");
        }
        
    }

    void OnTriggerExit (Collider other)
    {
        IReactsToWater boolInterface = other.GetComponent<IReactsToWater>();
        if (boolInterface != null)
        {
            boolInterface.IsWet = false; 
            Debug.Log("Splosh!!!");
        }
    }
    
}
