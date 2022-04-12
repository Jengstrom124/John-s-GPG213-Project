using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishModel : MonoBehaviour
{
    Neighbours neighbours;
    TurnTowards turnTowards;

    [Header("Reference Only:")]
    public bool isPlayerFish;
    public bool neighbourDebugColour = false;
    bool eventCalled = false;

    public event Action<GameObject> onPlayerFishEvent;
    public event Action onFishChangeEvent;

    // Start is called before the first frame update
    void Awake()
    {
        neighbours = GetComponent<Neighbours>();
        turnTowards = GetComponent<TurnTowards>();
    }

    private void Start()
    {
        neighbours.newNeighbourEvent += CheckForShark;
        neighbours.neighbourLeaveEvent += SharkExit;
        onFishChangeEvent += UpdateDebug;
    }

    void UpdateDebug()
    {
        neighbourDebugColour = false;
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
                onFishChangeEvent?.Invoke();
            }
        }
    }

    void CheckForShark(GameObject other)
    {
        if (other.GetComponent<SharkModel>() != null)
        {
            turnTowards.target = -other.transform.position;
        }
    }

    void SharkExit(GameObject other)
    {
        if (other.GetComponent<SharkModel>() != null)
        {
            turnTowards.target = Vector3.zero;
        }
    }
}
