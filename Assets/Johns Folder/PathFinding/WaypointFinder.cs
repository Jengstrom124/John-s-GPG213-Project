using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFinder : MonoBehaviour
{
    public WorldScanner worldScanner;

    public Vector2 randomNodePos;
    
    public Node RandomNodePosition()
    {
        randomNodePos = new Vector2(Random.Range(0, (int)worldScanner.gridSize.x), Random.Range(0, (int)worldScanner.gridSize.y));

        if(worldScanner.gridNodeReferences[(int)randomNodePos.x, (int)randomNodePos.y].isBlocked)
        {
            randomNodePos = new Vector2(Random.Range(0, (int)worldScanner.gridSize.x), Random.Range(0, (int)worldScanner.gridSize.y));
        }

        return worldScanner.gridNodeReferences[(int)randomNodePos.x, (int)randomNodePos.y];
    }
}
