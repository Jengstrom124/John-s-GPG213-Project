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

    public StateBase flyingState;
    public StateBase swoopState;
    public StateBase fleeState;

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

    
    /*
    RaycastHit CheckWhatsInFrontOfMe()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        Debug.DrawRay(ray.origin, ray.direction, Color.green, 2f);

        Physics.SphereCast(ray, 0.5f, out hit);

        return hit;
    }
    */
    

    //Different States
    public void FlyAway()
    {
        stateManager.ChangeState(fleeState);
    }

    public void SwoopChicken()
    {
        stateManager.ChangeState(swoopState);
    }

    public void Fly()
    {
        stateManager.ChangeState(flyingState);
    }
}
