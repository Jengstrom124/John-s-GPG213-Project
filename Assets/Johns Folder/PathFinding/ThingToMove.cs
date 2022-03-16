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

    Rigidbody rb;
    bool atEnd = false;
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
        FollowPath();
    }

    private void FixedUpdate()
    {
        RaycastHit hitinfo;
        hitinfo = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward, out hitinfo, maxRayLength, 255, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        //Debug.DrawLine(transform.position, hitinfo.point, Color.blue);

        if (hitinfo.collider)
        {
            pathBlocked = true;
        }
        else
        {
            pathBlocked = false;
        }
    }
    void FollowPath()
    {
        if (pathToFollow.Count > 0)
        {
            Vector2 pathPos = worldScanner.NodeToWorldPos(pathToFollow[0]);

            if (pathToFollow.Count > 1)
            {
                Vector2 pathPos2 = worldScanner.NodeToWorldPos(pathToFollow[1]);

                if (pathPos2.x != transform.position.x && pathPos2.y != transform.position.y)
                {
                    pathPos = pathPos2;
                    pathToFollow.Remove(pathToFollow[0]);
                }
            }

            transform.LookAt(new Vector3(pathPos.x, 5, pathPos.y));

            if (pathPos == new Vector2(transform.position.x, transform.position.z))
            {
                pathToFollow.Remove(pathToFollow[0]);
            }
            
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pathPos.x, 5, pathPos.y), speed);
            
        }
        else if (!atEnd)
        {
            atEnd = true;
            atEndNodeEvent?.Invoke(transform, waypoint.RandomNodePosition());
        }
    }

    void GeneratePathList(List<Node> path)
    {
        pathToFollow = path;
        atEnd = false;
    }
}
