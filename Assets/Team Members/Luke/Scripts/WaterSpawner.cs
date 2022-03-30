using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterSpawner : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	public Terrain _terrain;
	private TerrainData _terrainData;
	public GameObject coralPrefab;

	public float threshold = 0.45f;
	public float frequency = 15f;
	public int xOffset;
	public int yOffset;

	private IEnumerator Timer(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Debug.Log("time");
		FunctionLoop();
	}

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
			float value = Mathf.PerlinNoise(xCoord * frequency + xOffset, yCoord * frequency + yOffset);
			Debug.Log(value);
			//if statement
			if (value > threshold)
			{
				Vector3 worldPosition = new Vector3(x, _terrainData.GetHeight(x, y), y) + _terrain.transform.position;
				Instantiate(coralPrefab, worldPosition, Quaternion.identity, transform);
			}
		}
	}

	void Start()
	{
		_terrainData = terrainGenerator.terrainDataForRandomExample;
		xOffset = Random.Range(-1000, 1000);
		yOffset = Random.Range(-1000, 1000);

	StartCoroutine(Timer(1f));
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
