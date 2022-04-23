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
    public LayerMask obstacleLayerMask;

    [Header("Extra Settings: ")]
    public float maxRayLength = 10f;
    public bool drawRaycasts;
    public bool usePathSmoothing = true;
    public bool test = false;

    [Header("References Only - Don't Touch")]
    public Vector2 finalDestinationPos;
    public Vector2 currentTargetPos;
    bool atEnd = false;
    bool clearPathToTarget = false;
    List<Node> pathToFollow = new List<Node>();

    //EVENTS:
    //Subscribe to destinationReachedEvent for reacting to when entity reaches destination
    public event Action destinationReachedEvent;

    //Used in TurnTowards for updating the turn towards target
    public event Action<Vector3> newTargetAssignedEvent;

    // Start is called before the first frame update
    void Start()
    {
        if(AStar.Instance != null)
        {
            //AStar.Instance.pathFoundEvent += GeneratePathList;

            //For testing a random waypoint whilst we have no way to set a waypoint
            //AStar.Instance.FindPath(myTransform, new Vector3(10, 0, 40));
        }
        else
        {
            Debug.Log("AStar Reference Missing");
        }

        John.TestController.destinationSelectedEvent += GetPathToDestination;
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
        if(finalDestinationPos != Vector2.zero && usePathSmoothing)
        {
            RaycastHit rayToTarget;
            rayToTarget = new RaycastHit();
            Physics.Raycast(myTransform.position, new Vector3(finalDestinationPos.x, myTransform.position.y, finalDestinationPos.y) - myTransform.position, out rayToTarget, maxRayLength, obstacleLayerMask.value, QueryTriggerInteraction.Ignore);

            if(drawRaycasts)
            {
               Debug.DrawRay(myTransform.position, (new Vector3(finalDestinationPos.x, 0, finalDestinationPos.y) - transform.position) * maxRayLength, Color.green);
            }

            if (!rayToTarget.collider)
            {
                if(!clearPathToTarget)
                {
                    currentTargetPos = finalDestinationPos;
                    newTargetAssignedEvent?.Invoke(new Vector3(finalDestinationPos.x, 0, finalDestinationPos.y));
                    clearPathToTarget = true;
                }
            }
        }
    }

    //Monitor when we reach our target path Pos
    void KeepTrackOfPath()
    {
        //While there is a path to follow
        if (pathToFollow.Count > 0)
        {
            //Once we reach the current path position
            if (Vector2.Distance(currentTargetPos, new Vector2(myTransform.position.x, myTransform.position.z)) < distanceThreshold)
            {
                //Check for next path target
                if(usePathSmoothing)
                {
                    GetNextOptimizedPathPoint();
                }
                else
                {
                    pathToFollow.Remove(pathToFollow[0]);
                    currentTargetPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[0]);
                    newTargetAssignedEvent?.Invoke(new Vector3(currentTargetPos.x, 0, currentTargetPos.y));
                }
            }
        }
    }

    //When distination is in line of sight - monitor only when our we reach the destination
    void CheckForDestinationOnly()
    {
        //Clear the path list as we can now head directly to the desired target position
        pathToFollow.Clear();

        //If our position == the targetPos we have reached the end
        if (Vector2.Distance(finalDestinationPos, new Vector2(myTransform.position.x, myTransform.position.z)) < distanceThreshold && !atEnd)
        {
            //Fire off event
            atEnd = true;
            destinationReachedEvent?.Invoke();

            //HACK for now just to reset turn towards value
            currentTargetPos = Vector2.zero;
            newTargetAssignedEvent?.Invoke(Vector3.zero);
        }
    }

    public void GeneratePathList(List<Node> path)
    {
        pathToFollow = path;
        atEnd = false;
        clearPathToTarget = false;

        //Assign first path pos as our targetPos
        currentTargetPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[0]);
        newTargetAssignedEvent?.Invoke(new Vector3(currentTargetPos.x, 0, currentTargetPos.y));

        finalDestinationPos = new Vector2(AStar.Instance.targetPos.x, AStar.Instance.targetPos.z);
    }

    public void ResetPathTracking()
    {
        pathToFollow.Clear();
        atEnd = false;
        clearPathToTarget = false;
        currentTargetPos = Vector2.zero;
        finalDestinationPos = Vector2.zero;
    }

    void GetNextOptimizedPathPoint()
    {
        //Loop through each remaining path and find a path position we can head straight to that is not blocked
        for (int i = 0; i < pathToFollow.Count; i++)
        {
            if (pathToFollow[i] != AStar.Instance.targetNode)
            {
                Vector2 tempPathPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[i]);

                RaycastHit hitTest;
                hitTest = new RaycastHit();
                Physics.Raycast(myTransform.position, new Vector3(tempPathPos.x, myTransform.position.y, tempPathPos.y) - myTransform.position, out hitTest, maxRayLength, obstacleLayerMask.value, QueryTriggerInteraction.Ignore);
                if (hitTest.collider)
                {
                    //Once a path position is blocked - stop the loop and follow the path we have found
                    currentTargetPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[0]);
                    newTargetAssignedEvent?.Invoke(new Vector3(currentTargetPos.x, 0, currentTargetPos.y));
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

    public void GetPathToDestination(Vector3 destination)
    {
        AStar.Instance.FindPath(myTransform, destination);
    }
}
