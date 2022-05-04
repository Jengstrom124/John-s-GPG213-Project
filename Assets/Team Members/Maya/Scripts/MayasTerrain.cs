using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MayasTerrain : MonoBehaviour
{
    // Start is called before the first frame update
    public TerrainGenerator terrainGenerator;
    public TerrainData mayaTerrainData;

    public float width;
    public float height;
    public float scale;
    public float border;

    public float xFreq;
    public float yFreq;

    public float xOffset = 100f;
    public float yOffset = 100f;
    public float maxY;
    public float maxX;

    void Awake()
    {

    }

    private void Update()
    {
        /*if (InputSystem.GetDevice<Keyboard>().spaceKey.isPressed)
        {
            LetsDoIt();
        }*/
    }


    void LetsDoIt()
    {
        width = 256;
        height = 256;
        scale = 25;
        border = 0.2f;
        xFreq = 0.25f;
        yFreq = 0.3f;
        xOffset = Random.Range(0f, 100000f);
        yOffset = Random.Range(0f, 100000f);
        
        terrainGenerator.calculateHeightCallback = MayasTerrainHeight;
        terrainGenerator.GenerateTerrain(mayaTerrainData);
    }
    float MayasTerrainHeight(int x, int y)
    {
        float xVal = (float) x / width * scale + xOffset;
        float yVal = (float) y / height * scale + yOffset;
        float finalVal = Mathf.PerlinNoise(xVal*xFreq, yVal*yFreq);
        
        if (!(xVal < border+xOffset || yVal < border+yOffset || xVal > ((scale+xOffset) - border) || yVal > ((scale+yOffset) - border)))
        {
            if (finalVal < 0.575f)
                
            {
                //flattens the bottom of the water to 0
                finalVal = 0; //value * 0.9f; // - this bit was used for creating a sharp drop off from the beaches, but shark could still get caught
            }
            return finalVal;
        }
        
        {
            return 1;
        }
    }
}
