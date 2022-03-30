using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KevinTerrainGen : MonoBehaviour
{
    public TerrainData terrainData;

    public int width = 256;
    public int height = 256;
    public int depth = 20;

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public float maxPeak = 20f;
    public float minPeak = -20f; 
    
    public delegate float CalculateHeightDelegate(int x, int y);

    public CalculateHeightDelegate calculateHeightCallback; 
    void Start()
    {
        GenerateTerrain(terrainData); 
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1; 
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (calculateHeightCallback != null)
                    //heights[x, y] = calculateHeightCallback(x, y);
                    heights[x, y] = Mathf.PerlinNoise(maxPeak, 0); 
                Debug.Log(heights); 
            }
        }

        return heights;
    }
    void Update()
    {
     
    }
}
