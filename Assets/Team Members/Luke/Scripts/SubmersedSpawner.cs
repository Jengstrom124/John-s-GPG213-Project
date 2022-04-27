using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SubmersedSpawner : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	/*public Terrain _terrain;*/
	private TerrainData _terrainData;
	public List<GameObject> coralPrefabs;

	public float threshold = 0.45f;
	public float frequency = 15f;
	public int xOffset;
	public int yOffset;
	public float fringe;

	private void FunctionLoop()
	{
		GameObject coral = Instantiate(new GameObject("Coral"), transform);
		for (int x = 0; x < terrainGenerator.width; x++)
		{
			for (int y = 0; y < terrainGenerator.height; y++)
			{
				SpawnCoral(x,y, coral);
			}
		}
	}
	
	private void SpawnCoral(int x, int y, GameObject parent)
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
					GameObject go = Instantiate(coralPrefabs[Random.Range(0, coralPrefabs.Count)], worldPosition, Quaternion.Euler(0, Random.Range(-179, 180),0), parent.transform);
					go.transform.position += new Vector3(Random.Range(-0.2f,0.2f),0,Random.Range(-0.2f,0.2f));
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
