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
    bool eventCalled = false;

    public static event Action<GameObject> onPlayerFishEvent;
    public static event Action<GameObject> onFishChangeEvent;

    PathTracker pathTracker;
    private void Awake()
    {
        pathTracker = GetComponent<PathTracker>();
    }
    private void Start()
    {
        pathTracker.destinationReachedEvent += AtDestinationReaction;
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

    void AtDestinationReaction()
    {
        Debug.Log("MADE IT!! - return to being a fish");
    }

    public void Selected()
    {
        throw new NotImplementedException();
    }

    public void Deselected()
    {
        throw new NotImplementedException();
    }

    public void SetDestination(Vector3 position)
    {
        throw new NotImplementedException();
    }
}
