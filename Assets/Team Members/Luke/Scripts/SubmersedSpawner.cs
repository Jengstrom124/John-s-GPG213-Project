using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SubmersedSpawner : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	/*public Terrain _terrain;*/
	private TerrainData _terrainData;
	public GameObject coralPrefab;

	public float threshold = 0.45f;
	public float frequency = 15f;
	public int xOffset;
	public int yOffset;
	public float fringe;
	

	private void FunctionLoop()
	{
		for (int x = 0; x < terrainGenerator.width; x++)
		{
			for (int y = 0; y < terrainGenerator.height; y++)
			{
				SpawnCoral(x,y);
			}
		}
	}
	
	private void SpawnCoral(int x, int y)
	{
		if (_terrainData.GetHeight(x, y) < 6)
		{
			float xCoord = (float) x / terrainGenerator.width;
			float yCoord = (float) y / terrainGenerator.height;
			if (!(xCoord < fringe || xCoord > 1 - fringe || yCoord < fringe || yCoord > 1 - fringe))
			{
				float value = Mathf.PerlinNoise(xCoord * frequency + xOffset, yCoord * frequency + yOffset);
				if (value > threshold)
				{
					Vector3 worldPosition = new Vector3(x, _terrainData.GetHeight(x, y), y)/* + _terrain.transform.position*/;
					Instantiate(coralPrefab, worldPosition, Quaternion.identity, transform);
				}
			}
		}
	}

	void Awake()
	{
		GetComponentInParent<LukeTerrain>().FinishSpawningEvent += MyStart;
	}
	
	private void MyStart()
	{
		_terrainData = terrainGenerator.terrainDataForRandomExample;
		xOffset = Random.Range(-1000, 1000);
		yOffset = Random.Range(-1000, 1000);
		FunctionLoop();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
