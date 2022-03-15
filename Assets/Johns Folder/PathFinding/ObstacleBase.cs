using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
    public void StoppedMoving(GameObject go)
    {
        WorldScanner.instance.ReScan(go);
    }
}
