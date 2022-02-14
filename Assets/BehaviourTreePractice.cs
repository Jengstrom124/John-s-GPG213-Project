using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreePractice : MonoBehaviour
{
    public int distance = 5;
    public int distanceThreshold = 10;
    public GameObject target;


    public bool DistanceChecker()
    {
        //Calculate my distance from the targets distance
        distance = (int)Vector3.Distance(transform.position, target.transform.position);

        if(distance < distanceThreshold)
        {
            //gameObject.transform.Rotate(0, distanceThreshold - distance, 0)
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Spin()
    {
        gameObject.transform.Rotate(0, distanceThreshold - distance, 0);
    }


}
