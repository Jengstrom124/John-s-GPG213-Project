using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int spawnCount;

    public bool spawnFacingForward;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
            newObject.transform.position = new Vector3(Random.Range(-50, 30), 50, Random.Range(-40, 30));

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
