using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishModel : FishBase, IRTS
{
    [Header("Reference Only:")]
    public bool isPlayerFish;
    public bool hasWaypoint;
    public bool neighbourDebugColour = false;

    public event Action<bool> onPlayerFishEvent;

    PathTracker pathTracker;
    private void Awake()
    {
        pathTracker = GetComponent<PathTracker>();
    }
    private void Start()
    {
        pathTracker.destinationReachedEvent += AtDestinationReaction;
    }

    void AtDestinationReaction()
    {
        Debug.Log("MADE IT!! - return to being a fish");
    }

    public void Selected()
    {
        onPlayerFishEvent?.Invoke(true);
    }

    public void Deselected()
    {
        onPlayerFishEvent?.Invoke(false);
    }

    public void SetDestination(Vector3 position)
    {
        pathTracker.GetPathToDestination(position);
    }
}
