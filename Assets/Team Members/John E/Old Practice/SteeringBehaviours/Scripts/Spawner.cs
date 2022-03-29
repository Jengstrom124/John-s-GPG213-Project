using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spawner : MonoBehaviour
{
    //Spawn Area
    public int xArea = 50;
    public int yArea = 50;
    public int height = 0;

    public GameObject objectToSpawn;
    public int spawnCount;

    public bool spawnFacingForward;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
            newObject.transform.position = new Vector3(Random.Range(transform.position.x, transform.position.x + xArea), height, Random.Range(transform.position.z, transform.position.z + yArea));

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
