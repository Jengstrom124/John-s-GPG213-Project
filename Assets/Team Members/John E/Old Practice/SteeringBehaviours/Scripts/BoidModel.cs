using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoidModel : MonoBehaviour
{
    SBManager manager;

    public GameObject feeler;
    public GameObject emergencyFeeler;

    [Tooltip("Use an ODD Count ONLY")]
    public int standardFeelerCount = 5;
    public int emergencyFeelerCount = 3;

    [Tooltip("Adjust the angle of all RayCasts")]
    public float fov = 20;

    List<GameObject> leftFeelers = new List<GameObject>();
    List<GameObject> rightFeelers = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        SpawnFeelers();
    }

    private void Update()
    {
        UpdateFeelerFOV();
    }

    void SpawnFeelers()
    {
        for (int i = 0; i < standardFeelerCount; i++)
        {
            //spawn a feeler
            GameObject newFeeler = Instantiate(feeler, this.transform);
            AvoidObstacle avoidObstacle = newFeeler.GetComponent<AvoidObstacle>();

            //first spawn
            if (i == 0)
            {
                avoidObstacle.myTurnDirection = AvoidObstacle.RayDirection.Straight;
                newFeeler.transform.localRotation = Quaternion.Euler(transform.forward);
            }

            //Setting up RayDirection based on even / odd 

            //even
            else if (i % 2 == 0)
            {
                avoidObstacle.myTurnDirection = AvoidObstacle.RayDirection.Left;
                leftFeelers.Add(newFeeler);
            }

            //odd numbers
            else if (i % 2 == 1)
            {
                avoidObstacle.myTurnDirection = AvoidObstacle.RayDirection.Right;
                rightFeelers.Add(newFeeler);
            }

            
            if(i < emergencyFeelerCount)
            {
                GameObject newEmergencyFeeler = Instantiate(emergencyFeeler, this.transform);
                AvoidObstacle emergencyAvoidObstacle = newEmergencyFeeler.GetComponent<AvoidObstacle>();

                if(i <= 1)
                {
                    emergencyAvoidObstacle.myTurnDirection = AvoidObstacle.RayDirection.Straight;
                    newEmergencyFeeler.transform.localRotation = Quaternion.Euler(transform.forward);
                }
                else if(i == 1)
                {
                    emergencyAvoidObstacle.myTurnDirection = AvoidObstacle.RayDirection.Left;
                    newEmergencyFeeler.transform.localRotation = Quaternion.Euler(0, -35f, 0);
                }
                else
                {
                    emergencyAvoidObstacle.myTurnDirection = AvoidObstacle.RayDirection.Right;
                    newEmergencyFeeler.transform.localRotation = Quaternion.Euler(0, 35f, 0);
                }
            }
        }
    }

    void UpdateFeelerFOV()
    {
        foreach (GameObject leftFeeler in leftFeelers)
        {
            leftFeeler.transform.localRotation = Quaternion.Euler(0, -fov * (leftFeelers.IndexOf(leftFeeler) + 1), 0);
        }

        foreach (GameObject rightFeeler in rightFeelers)
        {
            rightFeeler.transform.localRotation = Quaternion.Euler(0, fov * (rightFeelers.IndexOf(rightFeeler) + 1), 0);
        }
    }
}
