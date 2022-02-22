using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBase : MonoBehaviour
{
    public virtual Vector3 CalculateMove(List<GameObject> neighbours)
    {
        return Vector3.zero;
    }
}
