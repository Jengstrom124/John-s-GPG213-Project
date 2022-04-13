using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Neighbours : MonoBehaviour
{
    public List<GameObject> neighboursList = new List<GameObject>();
    public List<Collider> fishColliders = new List<Collider>();
    
    public static  event Action<GameObject> newNeighbourEvent;
    public static event Action<GameObject> neighbourLeaveEvent;

    private void OnTriggerEnter(Collider other)
    {
        //Don't add a neighbour if we collide with a vision cone (ie a boid vision cone behind us is not a neighbour)
        if(other == other.GetComponent<CapsuleCollider>())
        {

        }
        else
        {
            //Only add a neighnour if the physical neighbour is in our vision radius (boid behind us considers us a neighbour but they are not a neighbour to us)
            if (other.GetComponent<FishModel>() != null && !neighboursList.Contains(other.gameObject))
            {
                neighboursList.Add(other.gameObject);
            }
            
            newNeighbourEvent?.Invoke(other.gameObject);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        neighbourLeaveEvent.Invoke(other.gameObject);

        if (neighboursList.Contains(other.gameObject))
        {
            neighboursList.Remove(other.gameObject);
        }
        
        
        
    }
}
