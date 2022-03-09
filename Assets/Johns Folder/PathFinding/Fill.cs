using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fill : MonoBehaviour
{
    public WorldScanner worldScanner;
    public Transform seeker, target;
    public AStar aStar;

    private void Start()
    {
        StartCoroutine(FindPath(seeker.position, target.position));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Convert world positions to grid positions
        Node startNode = worldScanner.WorldToNodePos(startPos);
        Node targetNode = worldScanner.WorldToNodePos(targetPos);


        worldScanner.openList.Add(startNode);

        while (worldScanner.openList.Count > 0)
        {
            //Give slight delay to visualize what is happening
            yield return new WaitForSeconds(0.01f);

            //Initialise a current node - note, we do not care about fCost as this algorthim is designed to search everywhere
            Node currentNode = worldScanner.openList[0];

            //Remove current node from open list & add to closed list
            worldScanner.openList.Remove(currentNode);
            worldScanner.closedList.Add(currentNode);

            //stop when destination is eventually found
            if (currentNode == targetNode)
            {
                Debug.Log("DONE");
                StopAllCoroutines();

                //HACK - just using my aStar algo to draw the path
                aStar.enabled = true;
                this.enabled = false;
            }
            else
            {
                //Otherwise continue finding path to target using neighbours - loop through each neighbour
                foreach (Node neighbour in worldScanner.GetNeighbours(currentNode))
                {
                    //Only if the neighbour is blocked or has already been checked do we ignore it - again no need to check any costs
                    if (neighbour.isBlocked || worldScanner.closedList.Contains(neighbour))
                    {
                        //ignore this neighbour
                        continue;
                    }
                    else
                    {
                        //otherwise continue adding neighbours to the open list to continue searching
                        if (!worldScanner.openList.Contains(neighbour))
                        {
                            worldScanner.openList.Add(neighbour);
                        }
                    }
                }
            }

            
        }

    }
}
