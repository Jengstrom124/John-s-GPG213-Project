using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AntSpawner : MonoBehaviour
{
    //random.rotation

    //AREA TO SPAWN
    public Vector2Int xRange = new Vector2Int(0, 10);
    public Vector2Int zRange = new Vector2Int(0, 10);

    public int scale = 5;

    public GameObject[] objectsToSpawn;

    public float spawnThreshold = 0.5f;
    public float spawnHeight = 0f;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = xRange.x; x < xRange.y; x++)
        {
            for (int y = zRange.x; y < zRange.y; y++)
            {
                float xCoord = (float)x / (xRange.y - xRange.x) * scale;
                float yCoord = (float)y / (zRange.y - zRange.x) * scale;

                float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);

                if (perlinValue > spawnThreshold)
                {
                    GameObject randomObject = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
                    GameObject newObject = PrefabUtility.InstantiatePrefab(randomObject) as GameObject;
                    newObject.transform.position = new Vector3(x, spawnHeight, y);
                    newObject.transform.rotation = Random.rotation;
                }
            }
        }
    }
}
