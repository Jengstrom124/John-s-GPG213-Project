using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishModel : MonoBehaviour
{
    [Header("Reference Only:")]
    public bool isPlayerFish;
    public bool hasWaypoint;
    public bool neighbourDebugColour = false;
    bool eventCalled = false;

    public static event Action<GameObject> onPlayerFishEvent;
    public static event Action<GameObject> onFishChangeEvent;

    public event Action<bool, Transform> runFromPredatorEvent;
    public event Action<Vector3> onDestinationAssignedEvent;

    private void Start()
    {
        Neighbours.newNeighbourEvent += CheckForPredator;
        Neighbours.neighbourLeaveEvent += PredatorOutOfSight;
    }

    private void Update()
    {
        if (isPlayerFish)
        {
            if (!eventCalled)
            {
                onPlayerFishEvent?.Invoke(gameObject);
                eventCalled = true;
            }
        }
        else
        {
            if (eventCalled)
            {
                eventCalled = false;
                onFishChangeEvent?.Invoke(gameObject);
            }
        }
    }

    void CheckForPredator(GameObject other)
    {
        if (other.GetComponent<IPredator>() != null)
        {
            runFromPredatorEvent?.Invoke(true, other.transform);
        }
    }

    void PredatorOutOfSight(GameObject other)
    {
        if (other.GetComponent<IPredator>() != null)
        {
            runFromPredatorEvent?.Invoke(false, null);
        }
    }

    /*
    void SetDestination(Vector3 destination)
    {
        //Generate Path
        //aStar.FindPath(transform, WorldScanner.instance.WorldToNodePos(new Vector3(destination.x, 0, destination.y)));

        //Set turn towards target
        //turnTowards.targetXPos = pathTracker.currentTargetPos.x;
        Debug.Log("Look At: " + destination);
        onDestinationAssignedEvent?.Invoke(new Vector3(destination.x, 0, destination.y));
        //Debug.Log("DESTINATION SET");
    }
    */
}
