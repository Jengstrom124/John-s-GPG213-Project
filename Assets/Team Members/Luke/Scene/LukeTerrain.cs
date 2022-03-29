using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeTerrain : MonoBehaviour
{
	
	public int depth = 50;
	public int length = 250;
	public int width = 250;
	public float frequency = 4f;
	public float amplitude = 1f;
	
	private TerrainData GenerateTerrain(TerrainData terrainData)
	{
		terrainData.heightmapResolution = width + 1;
		
		terrainData.size = new Vector3(width, depth, length);
		
		terrainData.SetHeights(0,0,GenerateHeights());

		return terrainData;
	}
	
	float[,] GenerateHeights()
	{
		float[,] heights = new float[width, length];
		for (int i=10; i<width-10; i++)
		{
			for (int j=10; j < length-10; j++)
			{
				heights[i, j] = CalculateHeight(i, j);	
			}
		}

		return heights;
	}
	
	float CalculateHeight(int x, int y)
	{
		float xCoord = (float) x / width*frequency;
		float yCoord = (float) y / length*frequency;

		return amplitude * Mathf.PerlinNoise( xCoord,  yCoord);
	}
	
	// Start is called before the first frame update
    void Start()
    {
	    Terrain terrain = GetComponent<Terrain>();
	    terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
