using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LukeTerrain : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	
	public float scale = 20f;
	public float offsetX = 100f;
	public float offsetY = 100f;
	
	public float xOctave1Frequency = 4f;
	public float xOctave2Frequency = 40f;
	public float yOctave1Frequency = 4f;
	public float yOctave2Frequency = 40f;
	public float amplitudeOctave1 = 0.85f;
	public float amplitudeOctave2 = 0.1f;

	float CalculateHeight(int x, int y)
	{
		float xCoord = (float) x / terrainGenerator.width;
		float yCoord = (float) y / terrainGenerator.height;

		float octave1 = amplitudeOctave1 * Mathf.PerlinNoise( xCoord*xOctave1Frequency+offsetX,  yCoord*yOctave1Frequency+offsetY);
		float octave2 = amplitudeOctave2 * Mathf.PerlinNoise( xCoord*xOctave2Frequency+offsetX,  yCoord*yOctave2Frequency+offsetY);

		if (octave1 > 0.7f)
		{
			return octave1 + octave2;
		}
		else if(octave1<0.42)
		{
			return octave1 * 0.8f;
		}
		
		return octave1;
	}
	
	// Start is called before the first frame update

	void Start()
	{
		offsetX = Random.Range(-1000f,1000f);
		offsetY = Random.Range(-1000f,1000f);
		terrainGenerator.scale = scale;
		terrainGenerator.calculateHeightCallback = CalculateHeight;
		terrainGenerator.GenerateTerrain(terrainGenerator.terrainDataForRandomExample);
	}
	
	void Update()
    {

    }
}
