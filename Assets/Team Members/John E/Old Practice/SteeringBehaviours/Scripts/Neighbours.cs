using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Neighbours : MonoBehaviour
{
    public List<GameObject> neighbours;

    public List<Collider> fishColliders = new List<Collider>();
    
    public event Action<Collider> inVisionEvent;
    public event Action<Collider> outVisionEvent;


    private void OnTriggerEnter(Collider other)
    {
        //Don't add a neighbour if we collide with a vision cone (ie a boid vision cone behind us is not a neighbour)
        if(other == other.GetComponent<CapsuleCollider>())
        {
            Debug.Log("Skip");
        }
        else
        {
            //Only add a neighnour if the physical neighbour is in our vision radius (boid behind us considers us a neighbour but they are not a neighbour to us)
            if (other.GetComponent<BoidModel>() != null && !neighbours.Contains(other.gameObject))
            {
                neighbours.Add(other.gameObject);
            }
            else
            {
                inVisionEvent?.Invoke(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (neighbours.Contains(other.gameObject))
        {
            neighbours.Remove(other.gameObject);
        }
        else
        {
            outVisionEvent.Invoke(other);
        }
    }
}
