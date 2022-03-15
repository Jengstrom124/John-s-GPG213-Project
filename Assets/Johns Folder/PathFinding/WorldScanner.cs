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

    //public List<Obstacle> obstacles = new List<Obstacle>();
    Object[] obstacleArray;

    //Used for storing the closed nodes of an object on the map once it moves (so we can rescan those nodes and update them)
    List<Node> objectClosedNodes = new List<Node>();

    private void Awake()
    {
        CreateGrid();

        //Sub to all obstacle objects - tell the world scanner to rescan if they move
        //obstacles.Add(FindObjectsOfType<Obstacle>());

        obstacleArray = FindObjectsOfType<Obstacle>(); 

        foreach(Obstacle obstacle in obstacleArray)
        {
            obstacle.gameObject.GetComponent<Obstacle>().OnMovedEvent += ReScan;
        }
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

        //These coordinates are used for mappiping out the shape of the object
        float lowestXPos = objectClosedNodes[0].gridPos.x;
        float highestXPos = objectClosedNodes[1].gridPos.x;
        float lowestYPos = objectClosedNodes[0].gridPos.y;
        float highestYPos = objectClosedNodes[1].gridPos.y;

        foreach(Node n in objectClosedNodes)
        {
            float nXPos = n.gridPos.x;
            float nYPos = n.gridPos.y;

            //Loop through all nodes to find the correct node coordinates
            if(nXPos < lowestXPos)
            {
                lowestXPos = nXPos;
            }
            else if(nXPos > highestXPos)
            {
                highestXPos = nXPos;
            }
            else if(nYPos < lowestYPos)
            {
                lowestYPos = nYPos;
            }
            else if(nYPos > highestYPos)
            {
                highestYPos = nYPos;
            }

            //Rescan OLD Area Only - we can do this using our list of closed nodes
            if (Physics.CheckBox(new Vector3(nXPos, 0, nYPos), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacle))
            {
                //Something is still there
                gridNodeReferences[(int)nXPos, (int)nYPos].isBlocked = true;
            }
            else
            {
                gridNodeReferences[(int)nXPos, (int)nYPos].isBlocked = false;
            }
        }

        //Clear this list for future use
        objectClosedNodes.Clear();

        //Using the difference from both the lowest & highest coordinate values to FOR loop through the shape of the object
        int lowXDifferenceToCentre = (int)previousCentreNode.gridPos.x - (int)lowestXPos;
        int highXDifferenceToCentre = (int)highestXPos - (int)previousCentreNode.gridPos.x;

        int lowYDifferenceToCentre = (int)previousCentreNode.gridPos.y - (int)lowestYPos;
        int highYDifferenceToCentre = (int)highestYPos - (int)previousCentreNode.gridPos.y;

        //Scan NEW area ONLY - we can do this by using the centre node of the object and starting at its lowest x,y variables to its highest x,y variables
        for (int x = ((int)currentCentreNode.gridPos.x - lowXDifferenceToCentre) -1; x <= ((int)currentCentreNode.gridPos.x + highXDifferenceToCentre) +1; x++)
        {
            for (int y = ((int)currentCentreNode.gridPos.y - lowYDifferenceToCentre) -1; y <= ((int)currentCentreNode.gridPos.y + highYDifferenceToCentre) +1; y++)
            {
                //Check for obstacle
                if (Physics.CheckBox(new Vector3(x, 0, y), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacle))
                {
                    // Something is there
                    gridNodeReferences[x, y].isBlocked = true;
                }
                else
                {
                    gridNodeReferences[x, y].isBlocked = false;
                }
            }
        }
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
