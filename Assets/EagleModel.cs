using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleModel : MonoBehaviour
{
    //Replace with PlayerModel player
    public GameObject player;

    //Replace with ChickenModel chicken
    public GameObject chicken;

    public float radiusOfInterest = 10f;
    public float distance;

    public StateManager stateManager;

    public StateBase FlyingState;
    public StateBase SwoopState;
    public StateBase FleeState;

    public bool CheckMyRadius(GameObject objectOfInterest)
    {
        //Check my distance from target distance & return the difference
        distance = Vector3.Distance(transform.position, objectOfInterest.transform.position);

        if (distance < radiusOfInterest)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //TODO: Change these into States for a state machine
    public void FlyAway()
    {
        stateManager.ChangeState(FleeState);
    }

    public void SwoopChicken()
    {
        stateManager.ChangeState(SwoopState);
    }

    public void Fly()
    {
        stateManager.ChangeState(FlyingState);
    }
}
