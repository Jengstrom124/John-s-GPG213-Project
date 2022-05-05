using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class FishModel : FishBase, IRTS, IEdible
{
    public event Action<bool> onPlayerFishEvent;

    public float reduceForcesTimer = 2f;
    float defaultAlignForce;
    float defaultCohesionForce;

    PathTracker pathTracker;
    Align align;

    bool hasDestination = false;

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

    private void FixedUpdate()
    {
        if(hasDestination)
        {
        //Slowly reduce forces to 0 so the player fish leads the school and the school follows the player fish
            align.force = Mathf.Lerp(align.force, 0, reduceForcesTimer);
        }
    }

    void AtDestinationReaction()
    {
        Debug.Log("MADE IT!! - return to being a fish");
        hasDestination = false;
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
        if (pathTracker.pathGenerated)
        {
            pathTracker.ResetPathTracking();
        }

        pathTracker.GetPathToDestination(position);
        hasDestination = true;
    }

    public void GetEaten(IPredator eatenBy)
    {
        gameObject.SetActive(false);
    }

    public EdibleInfo GetInfo()
    {
        EdibleInfo edibleInfo = new EdibleInfo();
        edibleInfo.edibleType = EdibleType.Food;
        edibleInfo.amount = 5;

        return edibleInfo;
    }

    public void GotShatOut(IPredator shatOutBy)
    {
	    transform.DOPunchScale(new Vector3(2f, 2f, 2f), 0.5f);
    }
}
