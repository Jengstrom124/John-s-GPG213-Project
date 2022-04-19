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
}
