using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFinder : MonoBehaviour
{
    public WorldScanner worldScanner;

    public Vector2 randomNodePos;
    Vector2 previousNode;
    
    public Node RandomNodePosition()
    {
        GenerateRandomNode();

        previousNode = randomNodePos;
        return worldScanner.gridNodeReferences[(int)randomNodePos.x, (int)randomNodePos.y];
    }

    void GenerateRandomNode()
    {
        do
        {
            randomNodePos = new Vector2(Random.Range(0, (int)worldScanner.gridSize.x), Random.Range(0, (int)worldScanner.gridSize.y));
        }
        while (worldScanner.gridNodeReferences[(int)randomNodePos.x, (int)randomNodePos.y].isBlocked || randomNodePos == previousNode);
        
    }
}
