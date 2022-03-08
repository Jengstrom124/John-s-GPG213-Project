using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public WorldScanner worldScanner;
    public Transform seeker, target;

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Convert world positions to grid positions
        Node startNode = worldScanner.WorldToNodePos(startPos);
        Node targetNode = worldScanner.WorldToNodePos(targetPos);

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            //Initialise a current node
            Node currentNode = openList[0];

            //Once our openlist becomes populated with neighbours
            //Current node SHOULD ONLY be the node with the lowest f cost - check through each node in the list to find the node with the lowest f cost
            for(int i = 1; i < openList.Count; i++)
            {
                //if node in open list f cost is less then current node f cost OR if same f cost, get node with the lowest hCost
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].gCost < currentNode.gCost)
                {
                    //set new current node
                    currentNode = openList[i];
                }
            }

            //Remove current node from open list & add to closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //If at the target node - end the loop
            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            else
            {
                //Otherwise continue finding path to target using neighbours - loop through each neighbour
                foreach(Node neighbour in worldScanner.GetNeighbours(currentNode))
                {
                    if (neighbour.isBlocked || closedList.Contains(neighbour))
                    {
                        //ignore this neighbour
                        continue;
                    }
                    else
                    {
                        //Calculate hCost
                        neighbour.hCost = DistanceCheck(neighbour, targetNode);
                        currentNode.hCost = DistanceCheck(currentNode, targetNode);

                        //check if neighbours distance to target is less then my current distance OR if neighbour is not in openlist
                        if(neighbour.hCost < currentNode.hCost || !openList.Contains(neighbour))
                        {
                            //Using world positions to calculate gCost
                            Vector2 neighbourPos = worldScanner.NodeToWorldPos(neighbour);
                            Vector2 currentNodePos = worldScanner.NodeToWorldPos(currentNode);

                            //Calculate neighbour gCost - if neighbour is diagonal gCost is double
                            if (neighbourPos.y == currentNodePos.y || neighbourPos.y != currentNodePos.y && neighbourPos.x == currentNodePos.x)
                            {
                                neighbour.gCost = 7;
                            }
                            else
                            {
                                neighbour.gCost = 14;
                            }

                            //set parent to keep track of our path
                            neighbour.parent = currentNode;

                            if (!openList.Contains(neighbour))
                            {
                                openList.Add(neighbour);
                            }
                        }
                    }
                }
            }


        }

    }

    void RetracePath(Node start, Node end)
    {
        //List of nodes that create the path
        List<Node> path = new List<Node>();

        Node currentNode = end;

        while(currentNode != start)
        {
            //Create a path from the end node to the start node following the node parent trail
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        worldScanner.path = path;
    }

    int DistanceCheck(Node a, Node b)
    {
        return (int)Vector2.Distance(worldScanner.NodeToWorldPos(a), worldScanner.NodeToWorldPos(b));
    }
}
