using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScanner : MonoBehaviour
{
    public Vector2 gridSize;
    Node[,] gridNodeReferences;
    public LayerMask obstacle;

    private void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        gridNodeReferences = new Node[(int)gridSize.x, (int)gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                //For each grid position, create a new node and store node position
                gridNodeReferences[x,y] = new Node();
                gridNodeReferences[x, y].gridPos = new Vector2(x, y);

                //Check for obstacle
                if (Physics.CheckBox(new Vector3(x, 0, y), new Vector3(0.5f, 0, 0.5f), Quaternion.identity, obstacle))
                {
                    // Something is there
                    gridNodeReferences[x, y].isBlocked = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Stop constant null errors when not in play mode using null check
        if(gridNodeReferences != null)
        {
            //loop through each node and draw a cube for each grid position
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {                        
                    Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one);

                    //change colour to indicate if path is blocked or clear
                    if (gridNodeReferences[x,y].isBlocked)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                        //Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one);
                    }
                }
            }
        }
    }

}
