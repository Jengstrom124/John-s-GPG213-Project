using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishModel : FishBase, IRTS
{
    public event Action<bool> onPlayerFishEvent;

    PathTracker pathTracker;
    Align align;
    private void Awake()
    {
        pathTracker = GetComponent<PathTracker>();
        align = GetComponent<Align>();
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
