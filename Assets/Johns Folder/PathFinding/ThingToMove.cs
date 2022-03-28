using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThingToMove : MonoBehaviour
{
    public AStar aStar;
    public WorldScanner worldScanner;
    public WaypointFinder waypoint;

    public float speed;
    //public float turnSpeed;
    public float maxRayLength = 10f;

    Vector3 targetPos;

    Rigidbody rb;
    bool atEnd = false;
    bool clearPathToTarget = false;
    bool pathBlocked = false;

    List<Node> pathToFollow = new List<Node>();

    public event Action<Transform, Node> atEndNodeEvent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aStar.pathFoundEvent += GeneratePathList;
        
        aStar.FindPath(transform, waypoint.RandomNodePosition());
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = new Vector3(worldScanner.NodeToWorldPos(aStar.targetNode).x, 5, worldScanner.NodeToWorldPos(aStar.targetNode).y);

        //If our position == the targetPos we have reached the end
        if (targetPos == new Vector3(Mathf.RoundToInt(transform.position.x), 5, Mathf.RoundToInt(transform.position.z)) && !atEnd)
        {
            pathToFollow.Clear();
            atEnd = true;
            atEndNodeEvent?.Invoke(transform, waypoint.RandomNodePosition());
        }
    }

    private void FixedUpdate()
    {
        if(clearPathToTarget)
        {
            HeadToTarget();
        }
        else
        {
            FollowPath();
        }
    }

    void FollowPath()
    {
        //While there is a path to follow
        if (pathToFollow.Count > 0)
        {
            //Begin moving along the path
            Vector2 pathPos = worldScanner.NodeToWorldPos(pathToFollow[0]);

            transform.LookAt(new Vector3(pathPos.x, 5, pathPos.y));
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pathPos.x, 5, pathPos.y), speed);

            //Once we reach the current path position
            if (pathPos == new Vector2(transform.position.x, transform.position.z))
            {
                //check if there is a clear path to the target - else keep following the path
                RaycastHit rayToTarget;
                rayToTarget = new RaycastHit();
                Physics.Raycast(transform.position, targetPos - transform.position, out rayToTarget, maxRayLength);
                Debug.DrawRay(transform.position, (targetPos - transform.position) * maxRayLength, Color.green);

                if (!rayToTarget.collider)
                {
                    clearPathToTarget = true;
                }
                else
                {
                    //Check for the next path we can take
                    CheckPath();
                }
            }
        }
    }

    void HeadToTarget()
    {
        //Clear the path list as we can now head directly to the desired target position
        pathToFollow.Clear();

        transform.LookAt(targetPos);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
    }

    void GeneratePathList(List<Node> path)
    {
        pathToFollow = path;
        atEnd = false;
        clearPathToTarget = false;
    }

    void CheckPath()
    {
        //Loop through each remaining path and find a path position we can head straight to that is not blocked
        for (int i = 0; i < pathToFollow.Count; i++)
        {
            if (pathToFollow[i] != aStar.targetNode)
            {
                Vector2 tempPathPos = worldScanner.NodeToWorldPos(pathToFollow[i]);

                RaycastHit hitTest;
                hitTest = new RaycastHit();
                Physics.Raycast(transform.position, new Vector3(tempPathPos.x, 5, tempPathPos.y) - transform.position, out hitTest, maxRayLength);
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
        }
    }
}
