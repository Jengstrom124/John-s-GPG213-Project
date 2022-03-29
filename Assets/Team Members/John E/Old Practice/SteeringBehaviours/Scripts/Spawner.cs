using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spawner : MonoBehaviour
{
    //Spawn Area
    public Vector2 xCoords = new Vector2(0, 50);
    public Vector2 yCoords = new Vector2(0, 50);

    public GameObject objectToSpawn;
    public int spawnCount;

    public bool spawnFacingForward;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
            newObject.transform.position = new Vector3(Random.Range(xCoords.x, xCoords.y), 0, Random.Range(yCoords.x, yCoords.y));

            if(spawnFacingForward)
            {
                newObject.transform.rotation = Quaternion.Euler(transform.forward);
            }
            else
            {
                newObject.transform.rotation = Random.rotation;
            }

        }
    }
}
