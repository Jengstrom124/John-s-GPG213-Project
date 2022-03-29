using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occlusion : MonoBehaviour
{
    public int rayCount = 4;
    public float rayLength = 2.5f;
    public float fov = 2f; 

    public Transform test;

    private void Start()
    {
        CalculateOcclusion();
    }

    void CalculateOcclusion()
    {
        //foreach(Node node in WorldScanner.instance.totalOpenNodes)
        {
            spawnRayCasts(test);
        }
    }

    void spawnRayCasts(Transform position)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        for (int i = 0; i < rayCount; i++)
        {
            RaycastHit hit;
            hit = new RaycastHit();

            Physics.Raycast(position.position, new Vector3(0, 0, (fov * i) + 1), out hit, rayLength, 255, QueryTriggerInteraction.Ignore);

            Debug.DrawRay(position.position, new Vector3(0, 0, (fov * i) + 1) * rayLength, Color.green, 5f);

            hits.Add(hit);
        }
    }
}
