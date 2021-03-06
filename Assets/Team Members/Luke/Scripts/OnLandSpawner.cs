using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OnLandSpawner : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	/*public Terrain _terrain;*/
	private TerrainData _terrainData;
	public List<GameObject> treePrefabs;

	public float height = 14f;
	public float threshold = 0.95f;
	public float frequency = 12f;
	public int xOffset;
	public int yOffset;
	public float fringe;

	private void FunctionLoop()
	{
		GameObject trees = Instantiate(new GameObject("Trees"), transform);
		for (int x = 0; x < terrainGenerator.width; x++)
		{
			for (int y = 0; y < terrainGenerator.height; y++)
			{
				SpawnTrees(x,y, trees);
			}
		}
	}
	
	private void SpawnTrees(int x, int y, GameObject parent)
	{
		
		if (_terrainData.GetHeight(x, y) > height)
		{
			float xCoord = (float) x / terrainGenerator.width;
			float yCoord = (float) y / terrainGenerator.height;
			if (!(xCoord < fringe || xCoord > 1 - fringe || yCoord < fringe || yCoord > 1 - fringe))
			{
				float value = Mathf.PerlinNoise(xCoord * frequency + xOffset, yCoord * frequency + yOffset);
				if (value > threshold)
				{
					GameObject go;
					Vector3 worldPosition = new Vector3(x, _terrainData.GetHeight(x, y), y)/* + _terrain.transform.position*/;
					if (Random.Range(0f, 1f) > 0.5f)
					{
						go = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Count)], worldPosition, Quaternion.Euler(0, Random.Range(-179, 180),0), parent.transform);
						go.transform.localScale *= 2f;
						go.transform.position += new Vector3(Random.Range(-0.2f,0.2f),0,Random.Range(-0.2f,0.2f));
						go.isStatic = true;

					}
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
}
