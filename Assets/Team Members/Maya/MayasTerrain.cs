using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LetsDoIt();
        }
    }


    void LetsDoIt()
    {
        width = 256;
        height = 256;
        scale = 25;
        border = 0.25f;
        xFreq = 0.3f;
        yFreq = 0.5f;
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
            return finalVal;
        }
        
        {
            return 1;
        }
    }
}
