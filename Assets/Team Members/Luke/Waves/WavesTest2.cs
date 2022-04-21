using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesTest2 : MonoBehaviour
{
	public WavesTest1 master;

	public float offset = 64f;
	
	private Terrain terrain;
	private TerrainData terrainData;

	TerrainData GenerateTerrain(TerrainData terrainData)
	{
		terrainData.size = new Vector3(master.width, master.depth, master.length);
		
		terrainData.SetHeights(0,0,GenerateHeights());

		return terrainData;
	}

	float[,] GenerateHeights()
	{
		float[,] heights = new float[master.width, master.length];
		for (int i=0; i<master.width; i++)
		{
			for (int j=0; j < master.length; j++)
			{
				heights[i, j] = CalculateHeight(i, j);
			}
		}

		return heights;
	}

	float CalculateHeight(int x, int y)
	{
		float xCoord = (float) x / master.width*master.frequency;
		float zCoord = (float) y / master.length*master.frequency;

		return master.amplitude * 0.5f*Mathf.Sin(xCoord*master.xWeight-zCoord*master.zWeight + Time.time
			+ offset/master.length*master.frequency)+0.3f;
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
