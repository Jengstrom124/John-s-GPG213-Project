using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingToMove : MonoBehaviour
{
    public AStar aStar;
    public WorldScanner worldScanner;

    public float speed;
    Rigidbody rb;

    List<Node> pathToFollow = new List<Node>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aStar.pathFoundEvent += FollowPath;
    }

    // Update is called once per frame
    void Update()
    {        
        if(pathToFollow.Count > 0)
        {
            Vector2 pathPos = worldScanner.NodeToWorldPos(pathToFollow[0]);

            if(pathPos == new Vector2(transform.position.x, transform.position.z))
            {
                pathToFollow.Remove(pathToFollow[0]);
            }

            //rb.AddRelativeForce(Vector3.forward * speed);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pathPos.x, 5, pathPos.y), speed);

        }
    }

    void FollowPath(List<Node> path)
    {
        pathToFollow = path;
    }
}
