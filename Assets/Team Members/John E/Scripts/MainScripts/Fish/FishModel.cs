using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishModel : FishBase, IRTS
{
    public event Action<bool> onPlayerFishEvent;

    public float alignForceTimer = 2f;
    float defaultAlignForce;

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
        defaultAlignForce = align.force;
    }

    void AtDestinationReaction()
    {
        Debug.Log("MADE IT!! - return to being a fish");
        Deselected();
    }

    public void Selected()
    {
        onPlayerFishEvent?.Invoke(true);
    }

    public void Deselected()
    {
        onPlayerFishEvent?.Invoke(false);

        align.force = defaultAlignForce;

        //Reset Pathfinding
        pathTracker.ResetPathTracking();

        //HACK - need to reset the fish variable in the controller
        John.JohnRTSTestController.Instance.ResetController();
    }

    public void SetDestination(Vector3 position)
    {
        pathTracker.GetPathToDestination(position);

        Mathf.Lerp(align.force, 0, alignForceTimer * Time.deltaTime);
    }
}
