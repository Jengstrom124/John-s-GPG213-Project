using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	private TerrainData _terrainData;

	public int[,] coords;
	// Start is called before the first frame update
    
	void Start()
	{
		_terrainData = terrainGenerator.terrainDataForRandomExample;
		coords = new int [10,10];
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
