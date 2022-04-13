using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishModel : MonoBehaviour
{
    //Neighbours neighbours;
    TurnTowards turnTowards;

    [Header("Reference Only:")]
    public bool isPlayerFish;
    public bool hasWaypoint;
    public bool neighbourDebugColour = false;
    bool eventCalled = false;

    public static event Action<GameObject> onPlayerFishEvent;
    public static event Action<GameObject> onFishChangeEvent;

    // Start is called before the first frame update
    void Awake()
    {
        //neighbours = GetComponent<Neighbours>();
        turnTowards = GetComponent<TurnTowards>();
    }

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
            turnTowards.target = -other.transform.position;
        }
    }

    void PredatorOutOfSight(GameObject other)
    {
        if (other.GetComponent<IPredator>() != null)
        {
            turnTowards.target = Vector3.zero;
        }
    }
}
