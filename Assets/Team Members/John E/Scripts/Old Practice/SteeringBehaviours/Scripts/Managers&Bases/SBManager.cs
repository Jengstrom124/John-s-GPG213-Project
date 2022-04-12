using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBManager : MonoBehaviour
{
    public List<SteeringBase> behaviours;

    MoveForwards moveForwards;

    //Keeping track of feelers
    public AvoidObstacle[] feelers;
    public List<AvoidObstacle> activeFeelers = new List<AvoidObstacle>();
    public List<AvoidObstacle> activeEmergencyFeelers = new List<AvoidObstacle>();
    public bool emergencyFeelerInUse = false;

    // Start is called before the first frame update
    void Start()
    {
        feelers = GetComponentsInChildren<AvoidObstacle>();
        moveForwards = GetComponent<MoveForwards>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForActiveFeelers();

        //UseClosestDistanceSpeed();

        if(activeFeelers.Count <= 0)
        {
            moveForwards.speed = Mathf.Lerp(moveForwards.speed, moveForwards.maxSpeed, 3f);
        }
    }

    void CheckForActiveFeelers()
    {
        foreach (AvoidObstacle feeler in feelers)
        {
            if(feeler.feelerActive && !activeFeelers.Contains(feeler))
            {
                activeFeelers.Add(feeler);
            }
            else if(!feeler.feelerActive && activeFeelers.Contains(feeler))
            {
                activeFeelers.Remove(feeler);
            }
        }
    }

    void UseClosestDistanceSpeed()
    {
        if(activeEmergencyFeelers.Count >= 1)
        {
            moveForwards.speed = -5f;
        }
        else if (activeFeelers.Count == 1)
        {
            activeFeelers[0].updateSpeed = true;
        }
        else if (activeFeelers.Count >= 2)
        {
            for (int i = 1; i < activeFeelers.Count; i++)
            {
                if (!activeFeelers[i].isEmergencyFeeler && activeFeelers[i].distance < activeFeelers[i - 1].distance)
                {
                    activeFeelers[i].updateSpeed = true;
                    activeFeelers[i - 1].updateSpeed = false;
                }
            }
        }
    }
}
