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

    public float offsetX = 100f;
    public float offsetY = 100f;


    // Start is called before the first frame update
    void Start()
    {
        width = 256;
        height = 256;
        scale = 20;
        offsetX = Random.Range(0f, 1000000f);
        offsetY = Random.Range(0f, 1000000f);
        
        terrainGenerator.calculateHeightCallback = CalculateHeight;
        terrainGenerator.GenerateTerrain(ollieTerrainData);

        //heightsArray = ollieTerrainData.GetHeights(0,0,(int)width,(int)height);
        //heightsArray = ollieTerrainData.GetHeights(0,0,(int)256,(int)256);
    }
    
    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}