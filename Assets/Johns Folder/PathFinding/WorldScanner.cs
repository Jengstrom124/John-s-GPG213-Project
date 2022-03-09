using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScanner : MonoBehaviour
{
    public Transform startPos, endPos;
    Node startNode, endNode;
    public Vector2 gridSize;
    Node[,] gridNodeReferences;
    public LayerMask obstacle;

    public bool constantScan = false;

    public List<Node> path = new List<Node>();
    public List<Node> openList = new List<Node>();
    public List<Node> closedList = new List<Node>();

    public List<GameObject> obstacles = new List<GameObject>();

    private void Awake()
    {
        CreateGrid();

        //Sub to all obstacle objects - tell the world scanner to rescan if they move
        //obstacles.Add(FindObjectsOfType<Obstacle>());
        foreach(GameObject obstacle in obstacles)
        {
            Debug.Log("Subscribe");
        }
    }

    void Update()
    {
        if(constantScan)
        {
            CreateGrid();
        }
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
                if (Physics.CheckBox(new Vector3(x, 0, y), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacle))
                {
                    // Something is there
                    gridNodeReferences[x, y].isBlocked = true;
                }
            }
        }
    }

    public Node WorldToNodePos(Vector3 worldPos)
    {
        int x;
        int y;

        if((worldPos.x <= gridSize.x && worldPos.x >= 0) && (worldPos.z <= gridSize.y && worldPos.z >= 0))
        {
            x = (int)worldPos.x;
            y = (int)worldPos.z;
            return gridNodeReferences[x, y];

        }
        else
        {
            Debug.Log("Object World Position is outside of Grid Space");
            return null;
        }
    }

    public Vector2 NodeToWorldPos(Node node)
    {
        float x = node.gridPos.x;
        float y = node.gridPos.y;

        return new Vector2(x, y);
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        //Check in a 3 by 3 grid for all neighbours
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                //we are the centre grid - skip us
                if(x == 0 && y == 0)
                {
                    continue;
                }
                else
                {
                    //check to make neighbour is within the grid
                    int checkX = (int)node.gridPos.x + x;
                    int checkY = (int)node.gridPos.y + y;

                    if((checkX >= 0 && checkX <= gridSize.x -1) && (checkY >= 0 && checkY <= gridSize.y -1))
                    {
                        neighbours.Add(gridNodeReferences[checkX, checkY]);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        return neighbours;
    }

    private void OnDrawGizmos()
    {
        //Stop constant null errors when not in play mode using null check
        if(gridNodeReferences != null)
        {        
            startNode = WorldToNodePos(startPos.position);
            endNode = WorldToNodePos(endPos.position);

            //loop through each node and draw a cube for each grid position
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {                        

                    //change colour to indicate if path is blocked or clear
                    if(gridNodeReferences[x,y] == startNode)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    else if(gridNodeReferences[x,y] == endNode)
                    {
                        Gizmos.color = Color.grey;
                    }
                    else if (gridNodeReferences[x,y].isBlocked)
                    {
                        Gizmos.color = Color.red;
                    }
                    else if(gridNodeReferences[x, y] != startNode)
                    {
                        Gizmos.color = Color.green;
                    }

                    if(path != null)
                    {
                        if(path.Contains(gridNodeReferences[x, y]) && gridNodeReferences[x,y] != endNode)
                        {
                            Gizmos.color = Color.black;
                        }
                    }

                    if (openList != null)
                    {
                        if (openList.Contains(gridNodeReferences[x, y]) && gridNodeReferences[x, y] != endNode && gridNodeReferences[x, y] != startNode)
                        {
                            Gizmos.color = Color.white;
                        }
                    }

                    if (closedList != null)
                    {
                        if (closedList.Contains(gridNodeReferences[x, y]) && gridNodeReferences[x, y] != endNode && gridNodeReferences[x, y] != startNode)
                        {
                            Gizmos.color = Color.blue;
                        }
                    }

                    //Draw after to prevent weird offset
                    Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one);

                }
            }
        }
    }

}
