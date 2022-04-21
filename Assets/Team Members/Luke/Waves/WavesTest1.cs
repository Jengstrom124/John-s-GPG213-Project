using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesTest1 : MonoBehaviour
{
	public int depth = 20;
	public int length = 32;
	public int width = 32;
	public float frequency = 20f;
	public float amplitude = 0.03f;
	public float xWeight = 1f;
	public float zWeight = 1f;

	private Terrain terrain;
	private TerrainData terrainData;

	TerrainData GenerateTerrain(TerrainData terrainData)
	{
		terrainData.size = new Vector3(width, depth, length);
		
		terrainData.SetHeights(0,0,GenerateHeights());

		return terrainData;
	}

	float[,] GenerateHeights()
	{
		float[,] heights = new float[width, length];
		for (int i=0; i<width; i++)
		{
			for (int j=0; j<length; j++)
			{
				heights[i, j] = CalculateHeight(i, j);
			}
		}

		return heights;
	}

	float CalculateHeight(int x, int y)
	{
		float xCoord = (float) x / width*frequency;
		float zCoord = (float) y / length*frequency;

		return amplitude * 0.5f*Mathf.Sin(xCoord*xWeight-zCoord*zWeight + Time.time)+0.3f;
	}


	// Start is called before the first frame update
	void Start()
	{
		terrain = GetComponent<Terrain>();
		terrainData = terrain.terrainData;
		terrainData = GenerateTerrain(terrainData);
	}

	private void Update()
	{
		GenerateHeights();
		terrainData.SetHeights(0,0,GenerateHeights());
	}
}
