using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class mayaSpawner : MonoBehaviour
{
    //Spawn Area
    [Header("Custom Spawn Area (Not using SpawnerPos)")]
    public Vector2Int xMinYMin = new Vector2Int(0,0);
    public Vector2Int xMaxYMax = new Vector2Int(50,50);

    [Header("Adjust Only When Using Spawner Pos (Adds to current position)")]
    public int xMax = 50;
    public int yMax = 50;

    [Header("Configurations:")]
    public GameObject objectToSpawn;
    public int spawnCount;
    public int spawnHeight = 0;

    [Header("Options:")]
    public bool spawnFacingForward;
    public bool useSpawnerPosition;
    public bool autoSpawnAtStart;


    // Start is called before the first frame update
    void Start()
    {
        if (autoSpawnAtStart)
            SpawnStuff();
    }

    public void SpawnStuff()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;

            if (useSpawnerPosition)
            {
                newObject.transform.position =
                    new Vector3(Random.Range(transform.position.x, transform.position.x + xMax), spawnHeight,
                        Random.Range(transform.position.z, transform.position.z + yMax));
            }
            else
            {
                newObject.transform.position = new Vector3(Random.Range(xMinYMin.x, xMaxYMax.x), spawnHeight,
                    Random.Range(xMinYMin.y, xMaxYMax.y));
            }

            if (spawnFacingForward)
            {
                newObject.transform.rotation = Quaternion.Euler(transform.forward);
            }
            else
            {
                newObject.transform.rotation = new Quaternion(newObject.transform.rotation.x, Random.Range(1, 359), newObject.transform.rotation.z, 0);
            }

        }
    }
}