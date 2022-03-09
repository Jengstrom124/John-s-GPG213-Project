using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fill : MonoBehaviour
{
    public WorldScanner worldScanner;
    public Transform seeker, target;

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
            yield return new WaitForSeconds(0.01f);
            //Initialise a current node
            Node currentNode = worldScanner.openList[0];

            //Remove current node from open list & add to closed list
            worldScanner.openList.Remove(currentNode);
            worldScanner.closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                Debug.Log("DONE");
                StopAllCoroutines();
            }
            else
            {
                //Otherwise continue finding path to target using neighbours - loop through each neighbour
                foreach (Node neighbour in worldScanner.GetNeighbours(currentNode))
                {
                    if (neighbour.isBlocked || worldScanner.closedList.Contains(neighbour))
                    {
                        //ignore this neighbour
                        continue;
                    }
                    else
                    {

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
