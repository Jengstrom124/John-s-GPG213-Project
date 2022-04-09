using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldScanner : MonoBehaviour
{
    //Singleton Declaration
    public static WorldScanner instance;

    //General Variables
    public Transform startPos, endPos;
    public Node startNode, endNode;
    public Vector2 gridSize;
    public Node[,] gridNodeReferences;
    public LayerMask obstacle;

    [Header("Option")]
    public bool constantScan = false;

    //List for visualizing nodes only
    public List<Node> path = new List<Node>();
    public List<Node> openList = new List<Node>();
    public List<Node> closedList = new List<Node>();
    public List<Node> totalOpenNodes = new List<Node>();

    //public List<ObstacleBase> dynamicObstacles = new List<ObstacleBase>();
    Obstacle[] obstacleArray;

    //Used for storing the closed nodes of an object on the map once it moves (so we can rescan those nodes and update them)
    List<Node> objectClosedNodes = new List<Node>();

    public event Action onReScanEvent;

    private void Awake()
    {
        instance = this;

        CreateGrid();

        //Sub to all obstacle objects in the world
        //obstacleArray = FindObjectsOfType<Obstacle>(); 

        //foreach(Obstacle obstacle in obstacleArray)
        {
            //obstacle.gameObject.GetComponent<Obstacle>().OnMovedEvent += ReScan;
        }

        //Sub to single static event
        //StaticEvents.ReScanEvent += ReScan;
    }

    void Update()
    {
        if(constantScan)
        {
            CreateGrid();
        }
    }

    public void CreateGrid()
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
                else
                {
                    totalOpenNodes.Add(gridNodeReferences[x, y]);
                }
            }
        }
    }

    public void ReScan(GameObject go)
    {
        Vector3 objectPreviousPos = go.GetComponent<Obstacle>().previousPos;
        Vector3 objectNewPos = go.transform.position;

        //Use this centre node as a reference for how big the object is and where it is on the map
        Node previousCentreNode = WorldToNodePos(objectPreviousPos);
        Node currentCentreNode = WorldToNodePos(objectNewPos);

        //Fill list with (blocked) neighbours of the previous centre node
        CheckForBlockedNeighbours(previousCentreNode);

        //Continue to iterate through that list until all previous blocked nodes have been found
        for(int i = 0; i < objectClosedNodes.Count; i++)
        {
            CheckForBlockedNeighbours(objectClosedNodes[i]);
        }

        foreach (Node n in objectClosedNodes)
        {
            float nXPos = n.gridPos.x;
            float nYPos = n.gridPos.y;

            //Rescan OLD Area Only - we can do this using our list of closed nodes
            if(objectClosedNodes != null)
            {
                if (Physics.CheckBox(new Vector3(nXPos, 0, nYPos), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacle))
                {
                    //Something is still there
                    gridNodeReferences[(int)nXPos, (int)nYPos].isBlocked = true;

                    RemoveNodeFromOpenNodeList(gridNodeReferences[(int)nXPos, (int)nYPos]);
                }
                else
                {
                    gridNodeReferences[(int)nXPos, (int)nYPos].isBlocked = false;

                    AddNodeToOpenNodeList(gridNodeReferences[(int)nXPos, (int)nYPos]);
                }
            }
           
        }

        //Clear this list for future use
        objectClosedNodes.Clear();

        //Object Bounds/Size
        Collider collider = go.GetComponent<Collider>();
        Vector3 minSize = collider.bounds.min;
        Vector3 maxSize = collider.bounds.max;

        //Scan NEW area ONLY - we can do this by using the bounds of the object
        for (int x = (int)minSize.x -1; x <= (int)maxSize.x +1; x++)
        {
            for (int y = (int)minSize.z -1; y <= (int)maxSize.z +1; y++)
            {
                //Make sure obstacle is within grid bounds
                if(((x + 1 <= gridSize.x && x + 1 >= 0) && (y + 1 <= gridSize.y && y + 1 >= 0)) && ((x <= gridSize.x && x >= 0) && (y <= gridSize.y && y >= 0)))
                {
                    if (Physics.CheckBox(new Vector3(x, 0, y), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacle))
                    {
                        // Something is there
                        gridNodeReferences[x, y].isBlocked = true;

                        RemoveNodeFromOpenNodeList(gridNodeReferences[x, y]);
                    }
                    else
                    {
                        gridNodeReferences[x, y].isBlocked = false;

                        AddNodeToOpenNodeList(gridNodeReferences[x, y]);
                    }
                }
                else
                {
                    Debug.Log(go.name + " Outside of Grid Space");
                }
                
            }
        }

        //Shoot an event to let pathfinding algorithms recalculate a path based on the new grid information
        onReScanEvent?.Invoke();
    }

    void CheckForBlockedNeighbours(Node node)
    {
        foreach(Node neighbour in GetNeighbours(node))
        {
            if(neighbour.isBlocked && !objectClosedNodes.Contains(neighbour))
            {
                objectClosedNodes.Add(neighbour);
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
                    //int checkX = (int)node.gridPos.x + x;
                    //int checkY = (int)node.gridPos.y + y;
                    if (node != null)
                    {
                        if (((int)node.gridPos.x + x >= 0 && (int)node.gridPos.x + x <= gridSize.x - 1) && ((int)node.gridPos.y + y >= 0 && (int)node.gridPos.y + y <= gridSize.y - 1))
                        {
                            neighbours.Add(gridNodeReferences[(int)node.gridPos.x + x, (int)node.gridPos.y + y]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        return neighbours;
    }

    void RemoveNodeFromOpenNodeList(Node node)
    {
        if (totalOpenNodes.Contains(node))
        {
            totalOpenNodes.Remove(node);
        }
    }

    void AddNodeToOpenNodeList(Node node)
    {
        if (!totalOpenNodes.Contains(node))
        {
            totalOpenNodes.Add(node);
        }
    }

    private void OnDrawGizmos()
    {
        //Stop constant null errors when not in play mode using null check
        if(gridNodeReferences != null)
        { 
            if(startNode != null)
                startNode = WorldToNodePos(startPos.position);
            //endNode = WorldToNodePos(endPos.position);

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
