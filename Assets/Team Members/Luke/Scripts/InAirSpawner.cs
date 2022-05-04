using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InAirSpawner : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	/*public Terrain _terrain;*/
	private TerrainData _terrainData;
	public List<GameObject> birdPrefabs;

	public float threshold = 0.9f;
	public float frequency = 30f;
	public int xOffset;
	public int yOffset;
	public float fringe;
	public int totalBirds;

	private void FunctionLoop()
	{
		GameObject birds = Instantiate(new GameObject("Birds"), transform);
		for (int i=0; i < totalBirds; i++)
		{
			int x = Random.Range(0, terrainGenerator.width);
			int y = Random.Range(0, terrainGenerator.height);
			SpawnBirds(x,y, birds);
		}
	}
	
	private void SpawnBirds(int x, int y, GameObject parent)
	{
		Vector3 worldPosition = new Vector3(x, 15f, y)/* + _terrain.transform.position*/;
		GameObject instantiate = Instantiate(birdPrefabs[Random.Range(0, birdPrefabs.Count)], worldPosition, Quaternion.Euler(0, Random.Range(-179, 180),0), parent.transform);
		instantiate.isStatic = true;
	}

	void Awake()
	{
		GetComponentInParent<LukeTerrain>().FinishSpawningEvent += MyStart;
		totalBirds = Random.Range(4, 8);
	}
	
	private void MyStart()
	{
		_terrainData = terrainGenerator.terrainDataForRandomExample;
		xOffset = Random.Range(-1000, 1000);
		yOffset = Random.Range(-1000, 1000);
		FunctionLoop();
	}
}

