using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScanner : MonoBehaviour
{
    public Vector2 gridSize;
    public Node[,] gridNodeReferences;

    private void Awake()
    {
        // Scan the real world starting at 0,0,0 (to be able to place the grid anywhere, add transform.position)

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (Physics.CheckBox(new Vector3(x * gridSize.x, 0, y * gridSize.y), new Vector3(gridSize.x, gridSize.y, gridSize.y), Quaternion.identity))
                {
                    // Something is there
                    gridNodeReferences[x, y].isBlocked = true;
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                //if (gridNodeReferences[x, y].isBlocked)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one);
                }
            }
        }
    }

}
