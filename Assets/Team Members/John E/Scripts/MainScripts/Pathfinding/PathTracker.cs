using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathTracker : MonoBehaviour
{
    [Header("Setup:")]
    [Tooltip("Transform to keep track of")]
    public Transform myTransform;
    [Tooltip("How close to the targetPos does the transform need to be")]
    public float distanceThreshold = 2f;

    [Header("Necessary Script References")]
    public AStar aStar;
    //public WorldScanner WorldScannerinstance;

    [Header("Extra Settings: ")]
    public float maxRayLength = 10f;
    public bool drawRaycasts;

    [Header("Test stuff only / Ignore")]
    public WaypointFinder waypoint;
    public bool randomWaypointTest;
    public event Action<Transform, Node> atEndNodeEvent;

    [Header("References Only - Don't Touch")]
    public Vector2 finalDestinationPos;
    public Vector2 currentTargetPos;
    bool atEnd = false;
    bool clearPathToTarget = false;
    List<Node> pathToFollow = new List<Node>();

    //Event for when entity reaches destination
    public event Action destinationReachedEvent;

    public event Action<Vector2> newTargetAssignedEvent;

    // Start is called before the first frame update
    void Start()
    {
        aStar.pathFoundEvent += GeneratePathList;
        
        if(randomWaypointTest)
        {
            aStar.FindPath(transform, waypoint.RandomNodePosition());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(clearPathToTarget)
        {
            CheckForDestinationOnly();
        }
        else
        {
            KeepTrackOfPath();
        }
    }

    private void FixedUpdate()
    {
        //check if there is a clear path to the target
        if(finalDestinationPos != null)
        {
            RaycastHit rayToTarget;
            rayToTarget = new RaycastHit();
            Physics.Raycast(myTransform.position, new Vector3(finalDestinationPos.x, 0, finalDestinationPos.y) - myTransform.position, out rayToTarget, maxRayLength);

            if(drawRaycasts)
            {
               Debug.DrawRay(myTransform.position, (new Vector3(finalDestinationPos.x, 0, finalDestinationPos.y) - transform.position) * maxRayLength, Color.green);
            }

            if (!rayToTarget.collider)
            {
                clearPathToTarget = true;
            }
        }
    }

    //Monitor when we reach our target path Pos
    void KeepTrackOfPath()
    {
        //While there is a path to follow
        if (pathToFollow.Count > 0)
        {
            //Assign first path pos as our targetPos
            currentTargetPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[0]);
            newTargetAssignedEvent?.Invoke(currentTargetPos);

            //Once we reach the current path position
            if (Vector2.Distance(currentTargetPos, new Vector2(myTransform.position.x, myTransform.position.z)) < distanceThreshold)
            {
                //Check for next path target
                CheckPath();
            }
        }
    }

    //When distination is in line of sight - monitor only when our we reach the destination
    void CheckForDestinationOnly()
    {
        //Clear the path list as we can now head directly to the desired target position
        pathToFollow.Clear();
        //currentTargetPos = finalDestinationPos;
        newTargetAssignedEvent?.Invoke(finalDestinationPos);

        //If our position == the targetPos we have reached the end
        if (Vector2.Distance(finalDestinationPos, new Vector2(myTransform.position.x, myTransform.position.z)) < distanceThreshold && !atEnd)
        {
            //Fire off event
            atEnd = true;
            destinationReachedEvent?.Invoke();


            //Ignore (for testing random waypoints)
            if(randomWaypointTest)
                atEndNodeEvent?.Invoke(transform, waypoint.RandomNodePosition());
        }
    }

    void GeneratePathList(List<Node> path)
    {
        pathToFollow = path;
        atEnd = false;
        clearPathToTarget = false;

        finalDestinationPos = new Vector2(WorldScanner.instance.NodeToWorldPos(aStar.targetNode).x, WorldScanner.instance.NodeToWorldPos(aStar.targetNode).y);
    }

    void CheckPath()
    {
        //Loop through each remaining path and find a path position we can head straight to that is not blocked
        for (int i = 0; i < pathToFollow.Count; i++)
        {
            if (pathToFollow[i] != aStar.targetNode)
            {
                Vector2 tempPathPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[i]);

                RaycastHit hitTest;
                hitTest = new RaycastHit();
                Physics.Raycast(myTransform.position, new Vector3(tempPathPos.x, 5, tempPathPos.y) - myTransform.position, out hitTest, maxRayLength);
                if (hitTest.collider)
                {
                    //Once a path position is blocked - stop the loop and follow the path we have found
                    return;
                }
                else
                {
                    //Remove each node path from the list that we do not want to travel to
                    pathToFollow.Remove(pathToFollow[i]);
                }
            }
            else
            {
                clearPathToTarget = true;
            }
        }
    }
}
