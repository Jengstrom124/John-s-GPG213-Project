using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;

public class LukeTerrain : MonoBehaviour
{
	public TerrainGenerator terrainGenerator;
	
	public float offsetX = 100f;
	public float offsetY = 100f;
	public float fringe = 0.05f;
	
	public float xOctave1Frequency = 4f;
	public float xOctave2Frequency = 40f;
	public float yOctave1Frequency = 4f;
	public float yOctave2Frequency = 40f;
	public float amplitudeOctave1 = 0.85f;
	public float amplitudeOctave2 = 0.1f;
	public float dropOffHeight = 0.42f;
	public float highLandHeight = 0.7f;

	//public float[,] previousHeights;

	private IEnumerator Timer()
	{
		yield return new WaitForSeconds(0.5f);
		FinishSpawningEvent?.Invoke();
	}
	
	float CalculateHeight(int x, int y)
	{
		float xCoord = (float) x / terrainGenerator.width;
		float yCoord = (float) y / terrainGenerator.height;

		if (x == terrainGenerator.width-1 && y == terrainGenerator.height-1)
		{
			StartCoroutine(Timer());
		}
		
		float octave1 = amplitudeOctave1 * Mathf.PerlinNoise( xCoord*xOctave1Frequency+offsetX,  yCoord*yOctave1Frequency+offsetY);
		float octave2 = amplitudeOctave2 * Mathf.PerlinNoise( xCoord*xOctave2Frequency+offsetX,  yCoord*yOctave2Frequency+offsetY);
		
		if (!(xCoord < fringe || xCoord > 1 - fringe || yCoord < fringe || yCoord > 1 - fringe))
		{
			if (octave1 > highLandHeight)
			{
				return octave1 + octave2;
			}
			
			if(octave1 < dropOffHeight)
			{
				return octave1 * 0.8f;
			}
		
			return octave1;
		}
		
		//fringe stuff
		if (xCoord < fringe)
		{
			if (yCoord < fringe && xCoord > yCoord)
			{
				return 0.4f*Mathf.Cos(yCoord*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt(yCoord/fringe);
			}
			if (yCoord > 1 - fringe && xCoord > 1-yCoord)
			{ 
				return 0.4f*Mathf.Cos((1-yCoord)*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt((1-yCoord)/fringe);
			}
			return 0.4f*Mathf.Cos(xCoord*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt(xCoord/fringe);
		}
		if (xCoord > 1 - fringe)
		{
			if (yCoord < fringe && 1-xCoord > yCoord)
			{
				return 0.4f*Mathf.Cos(yCoord*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt(yCoord/fringe);
			}
			if (yCoord > 1 - fringe && 1-xCoord > 1-yCoord)
			{
				return 0.4f*Mathf.Cos((1-yCoord)*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt((1-yCoord)/fringe);
			}
			return 0.4f*Mathf.Cos((1-xCoord)*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt((1-xCoord)/fringe);
		}
		if (yCoord < fringe)
		{
			return 0.4f*Mathf.Cos(yCoord*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt(yCoord/fringe);
		}
		return 0.4f*Mathf.Cos((1-yCoord)*Mathf.PI/fringe) + 0.4f + octave1*Mathf.Sqrt((1-yCoord)/fringe);
		
		//return previousHeights[x,y];
	}
	
	public delegate void FinishSpawningAction();
	public event FinishSpawningAction FinishSpawningEvent;

	// Start is called before the first frame update

	void Awake()
	{
		//previousHeights = terrainGenerator.terrainDataForRandomExample.GetHeights(0,0,terrainGenerator.width,terrainGenerator.height);
	}
	
	void Start()
	{
		offsetX = Random.Range(-1000f,1000f);
		offsetY = Random.Range(-1000f,1000f);
		GetComponentInChildren<SubmersedSpawner>().fringe = fringe;
		terrainGenerator.calculateHeightCallback = CalculateHeight;
		terrainGenerator.GenerateTerrain(terrainGenerator.terrainDataForRandomExample);
	}
	
	void Update()
    {

    }
}
