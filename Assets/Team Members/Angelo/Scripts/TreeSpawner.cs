using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public Terrain TargetTerrain;
    public int width = 256;
    public int height = 256;
    public GameObject test;
    public int TreeOdds = 25;

    public float testheight;

    public bool gentest = false;
    void Start()
    {
        gentest = true;   
    }

    void Update()
    {
        if (gentest)
        {
            gentest = false;
            Generate();
        }   
    }

    public void Generate()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (TargetTerrain.terrainData.GetHeight(x, y) > testheight)
                {
                    Debug.Log("Planted");
                    if(Random.Range(0,100) < TreeOdds)
                    {
                        GameObject spawn = Instantiate(test);
                        spawn.transform.position = new Vector3(x, TargetTerrain.terrainData.GetHeight(x, y), y);
                        float RandomScale = Random.Range(0.5f, 1.5f);
                        spawn.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
                    }                   
                }
            }
        }
    }
}
