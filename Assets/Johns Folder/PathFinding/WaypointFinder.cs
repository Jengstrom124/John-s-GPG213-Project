using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFinder : MonoBehaviour
{
    public WorldScanner worldScanner;
    
    public Node RandomNodePosition()
    {
        Vector2 pos = new Vector2(Random.Range(0, (int)worldScanner.gridSize.x), Random.Range(0, (int)worldScanner.gridSize.y));

        if(worldScanner.gridNodeReferences[(int)pos.x, (int)pos.y].isBlocked)
        {
            pos = new Vector2(Random.Range(0, (int)worldScanner.gridSize.x), Random.Range(0, (int)worldScanner.gridSize.y));
        }

        return worldScanner.gridNodeReferences[(int)pos.x, (int)pos.y];
    }
}
