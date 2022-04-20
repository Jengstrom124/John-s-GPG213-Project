using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OllieTerrain : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;
    public TerrainData ollieTerrainData;

    public float width;
    public float height;
    public float scale;
    public float border;

    public float xFrequency;
    public float yFrequency;

    public float offsetX = 100f;
    public float offsetY = 100f;


    // Keep as Awake to run before Birdzier takes width & height
    void Awake()
    {
        width = 256;
        height = 256;
        scale = 20;
        border = 0.5f;
        xFrequency = 0.25f;
        yFrequency = 0.25f;
        offsetX = Random.Range(0f, 1000000f);
        offsetY = Random.Range(0f, 1000000f);
        
        terrainGenerator.calculateHeightCallback = CalculateHeight;
        terrainGenerator.GenerateTerrain(ollieTerrainData);

        //heightsArray = ollieTerrainData.GetHeights(0,0,(int)width,(int)height);
        //heightsArray = ollieTerrainData.GetHeights(0,0,(int)256,(int)256);
    }
    
    float CalculateHeight(int x, int y)
    {
        //store value
        //multiply if below
        //return final value

        float xCoord = (float) x / width * scale + offsetX;
        float yCoord = (float) y / height * scale + offsetY;

      
        float value = Mathf.PerlinNoise(xCoord*xFrequency, yCoord*yFrequency);
        if (!(xCoord < border+offsetX || yCoord < border+offsetY || xCoord > ((scale+offsetX) - border) || yCoord > ((scale+offsetY) - border)))
        {
            if (value < 0.55f)
            {
                //flattens the bottom of the water to 0
                value = 0; //value * 0.9f; // - this bit was used for creating a sharp drop off from the beaches, but shark could still get caught
            }
            return value; //generates the hills/beaches
        }
        else //gives a flat, high border around the map. Should be modular based on scale
        {
            return 1;
        }
    }
}
