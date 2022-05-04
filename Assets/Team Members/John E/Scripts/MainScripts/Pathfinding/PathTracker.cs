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
    //public float maxRayLength = 10f;
    public bool drawRaycasts;
    public bool usePathSmoothing = true;

    [Header("References Only - Don't Touch")]
    public Vector2 finalDestinationPos;
    public Vector2 currentTargetPos;
    bool atEnd = false;
    bool clearPathToTarget = false;
    List<Node> pathToFollow = new List<Node>();
    List<Node> skipNodeList = new List<Node>();
    Node myDestinationNode;
    public bool pathGenerated = false;
    bool pathCleared = false;

    //EVENTS:
    //Subscribe to destinationReachedEvent for reacting to when entity reaches destination
    public event Action destinationReachedEvent;

    //Used in TurnTowards for updating the turn towards target
    public event Action<Vector3> newTargetAssignedEvent;

    // Update is called once per frame
    void Update()
    {
        if(pathGenerated)
        {
            if (clearPathToTarget)
            {
                CheckForDestinationOnly();
            }
            else
            {
                KeepTrackOfPath();
            }
        }
    }

    private void FixedUpdate()
    {
        //Ray to destination
        if(pathGenerated & !clearPathToTarget)
        {
            Ray ray = new Ray(myTransform.position, new Vector3(finalDestinationPos.x, myTransform.position.y - 2, finalDestinationPos.y) - myTransform.position);
            float rayDistance = Vector3.Distance(myTransform.position, new Vector3(finalDestinationPos.x, myTransform.position.y, finalDestinationPos.y));

            //If clear path to the end
            if (!Physics.Raycast(ray, rayDistance, obstacleLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                currentTargetPos = finalDestinationPos;
                newTargetAssignedEvent?.Invoke(new Vector3(finalDestinationPos.x, 0, finalDestinationPos.y));
                clearPathToTarget = true;
            }
        }
    }

    public void GeneratePathList(List<Node> path)
    {
        pathToFollow = path;
        pathCleared = false;
        atEnd = false;
        clearPathToTarget = false;
        finalDestinationPos = new Vector2(AStar.Instance.targetPos.x, AStar.Instance.targetPos.z);
        myDestinationNode = AStar.Instance.targetNode;

        if (usePathSmoothing)
        {
            GetNextOptimizedPath();
        }
        else
        {
            //Assign first path pos as our targetPos
            currentTargetPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[0]);
            newTargetAssignedEvent?.Invoke(new Vector3(currentTargetPos.x, 0, currentTargetPos.y));
        }

        pathGenerated = true;
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
                pathToFollow.Remove(pathToFollow[0]);

                if(!usePathSmoothing)
                {
                    currentTargetPos = WorldScanner.instance.NodeToWorldPos(pathToFollow[0]);
                    newTargetAssignedEvent?.Invoke(new Vector3(currentTargetPos.x, 0, currentTargetPos.y));
                }
                else
                {
                    GetNextOptimizedPath();
                }
            }
        }
    }

    //When distination is in line of sight - monitor only when our we reach the destination
    void CheckForDestinationOnly()
    {
        currentTargetPos = finalDestinationPos;

        //Clear the path list as we can now head directly to the desired target position
        if(!pathCleared)
        {
            pathToFollow.Clear();
            skipNodeList.Clear();
            pathCleared = true;
        }

        //If our position == the targetPos we have reached the end
        if (Vector2.Distance(finalDestinationPos, new Vector2(myTransform.position.x, myTransform.position.z)) < distanceThreshold && !atEnd)
        {
            //Fire off event
            atEnd = true;
            destinationReachedEvent?.Invoke();

            //HACK for now just to reset turn towards value
            currentTargetPos = Vector2.zero;
            finalDestinationPos = Vector2.zero;
            newTargetAssignedEvent?.Invoke(Vector3.zero);
            pathGenerated = false;
            pathCleared = false;
        }
    }

    void GetNextOptimizedPath()
    {
        foreach (Node path in pathToFollow)
        {
            if(path == myDestinationNode)
            {
                clearPathToTarget = true;
                newTargetAssignedEvent?.Invoke(new Vector3(finalDestinationPos.x, 0, finalDestinationPos.y));
                return;
            }

            Vector2 tempPathPos = WorldScanner.instance.NodeToWorldPos(path);
            Vector3 tempPathPosAsV3 = new Vector3(tempPathPos.x, myTransform.position.y, tempPathPos.y);

            //Calculating ray distance to not overshoot target and return inaccurate collision hits
            Ray ray = new Ray(myTransform.position, tempPathPosAsV3 - myTransform.position);
            float rayDistance = Vector3.Distance(myTransform.position, tempPathPosAsV3);

            //Ray to path pos - if clear remove from path list
            if (!Physics.Raycast(ray, rayDistance, obstacleLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                if(path != myDestinationNode)
                    skipNodeList.Add(path);
            }
            else
            {
                currentTargetPos = WorldScanner.instance.NodeToWorldPos(path);
                newTargetAssignedEvent?.Invoke(new Vector3(currentTargetPos.x, 0, currentTargetPos.y));
                break;
            }
        }

        foreach (Node path in skipNodeList)
        {
            if (pathToFollow.Contains(path))
            {
                pathToFollow.Remove(path);
            }
        }

        skipNodeList.Clear();
    }

    public void ResetPathTracking()
    {
        pathToFollow.Clear();
        skipNodeList.Clear();
        atEnd = false;
        clearPathToTarget = false;
        pathGenerated = false;
        currentTargetPos = Vector2.zero;
        finalDestinationPos = Vector2.zero;
        newTargetAssignedEvent?.Invoke(Vector3.zero);
    }

    public void GetPathToDestination(Vector3 destination)
    {
        AStar.Instance.FindPath(myTransform, destination);
    }
}
