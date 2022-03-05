using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isBlocked;
    public Vector2 gridPos;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node parent;
}
