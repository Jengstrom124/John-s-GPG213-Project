using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
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


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = Instantiate(objectToSpawn) as GameObject;

            if(useSpawnerPosition)
            {
                newObject.transform.position = new Vector3(Random.Range(transform.position.x, transform.position.x + xMax), spawnHeight, Random.Range(transform.position.z, transform.position.z + yMax));
            }
            else
            {
                newObject.transform.position = new Vector3(Random.Range(xMinYMin.x, xMaxYMax.x), spawnHeight, Random.Range(xMinYMin.y, xMaxYMax.y));
            }

            if (spawnFacingForward)
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
