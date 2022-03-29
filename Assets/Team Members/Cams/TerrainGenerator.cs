using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// This is for getting the coordinates that correspond to the terrain settings
// Good for plugging into Mathf.Perlin()
//	float xCoord = (float) x / width * scale + offsetX;
//	float yCoord = (float) y / height * scale + offsetY;


public class TerrainGenerator : MonoBehaviour
{
	public int width = 256; //x-axis of the terrain
	public int height = 256; //z-axis

	public int depth = 20; //y-axis

	public float scale = 20f;

	public float offsetX = 100f;
	public float offsetY = 100f;
	
	// Delegates are simply
	public delegate float CalculateHeightDelegate(int x, int y);
	public CalculateHeightDelegate calculateHeightCallback;
	
	[Header("Demo")]
	public bool randomHeightTest = false;
	public TerrainData terrainDataForRandomExample;
	
	private void Start()
	{
		if (randomHeightTest)
		{
			// Set the callback to a temp randomiser function
			calculateHeightCallback = DemoCalculateRandomHeight;
			GenerateTerrain(terrainDataForRandomExample);
		}
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
					heights[x, y] = calculateHeightCallback(x, y);
			}
		}

		return heights;
	}

	float DemoCalculateRandomHeight(int x, int y)
	{
		return Random.value;
	}
}