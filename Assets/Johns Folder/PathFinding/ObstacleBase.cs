using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
    public void Awake()
    {
        WorldScanner.instance.dynamicObstacles.Add(this);
    }
    public void StoppedMoving(GameObject go)
    {
        WorldScanner.instance.ReScan(go);
    }
}
