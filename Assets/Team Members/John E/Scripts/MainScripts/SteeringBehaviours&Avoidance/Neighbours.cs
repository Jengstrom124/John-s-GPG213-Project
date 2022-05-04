using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Neighbours : MonoBehaviour
{
    public Transform centreOfVisionPosition;
    public float visionRadius = 27f;

    public List<GameObject> neighboursList = new List<GameObject>();
    public Collider[] colliders;
    
    public event Action<GameObject> seePredatorEvent;
    public event Action<GameObject> neighbourLeaveEvent;

    private void Start()
    {
        StartCoroutine(CheckForNeighbours());
    }

    IEnumerator CheckForNeighbours()
    {
        //Physics.OverlapSphereNonAlloc(centreOfVisionPosition.position, visionRadius, colliders, 255, QueryTriggerInteraction.Ignore);
        colliders = Physics.OverlapSphere(centreOfVisionPosition.position, visionRadius, 255, QueryTriggerInteraction.Ignore);
        neighboursList.Clear();
        seePredatorEvent?.Invoke(null);

        foreach(Collider collider in colliders)
        {
            //Only add a neighbour if they are a fish and not us
            if (collider.GetComponent<FishBase>() != null && !neighboursList.Contains(collider.gameObject) && collider.gameObject != gameObject)
            {
                neighboursList.Add(collider.gameObject);
            }

            if(collider.GetComponent<IPredator>() != null)
            {
                seePredatorEvent?.Invoke(collider.gameObject);
            }
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(CheckForNeighbours());
    }
}
