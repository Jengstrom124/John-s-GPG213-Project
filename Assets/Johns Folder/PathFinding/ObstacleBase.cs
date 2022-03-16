using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
    Rigidbody rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();

        WorldScanner.instance.dynamicObstacles.Add(this);
    }

    public void StoppedMoving(GameObject go)
    {
        WorldScanner.instance.ReScan(go);
    }
}
