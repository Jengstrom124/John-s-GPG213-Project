using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tree = Unity.VisualScripting.Antlr3.Runtime.Tree.Tree;

public class KevinTerrainGen : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    public Terrain terrainData;
   
   

    public float frequencyX = 1f;
    public float frequencyY = 1f;
    public int yHeight = 20;
    
    public float seaLevel = 10f; 
    
    public GameObject sandPrefab;
    public GameObject treePrefab;
    public GameObject seaweedPrefab;
    public GameObject coralPrefab;
    
    private List<Vector3> currentPosition;
    
    void Start()
    {
        terrainGenerator.calculateHeightCallback = CalculateHeightCallback;
        terrainGenerator.GenerateTerrain(terrainGenerator.terrainDataForRandomExample);
        terrainGenerator.depth = yHeight;
        
        
        
        for (int x = 0; x < terrainGenerator.width; x++)
        {
            for (int z = 0; z < terrainGenerator.height; z++)
            {
                float perlinValue1 = Mathf.PerlinNoise(10f * (float) x / terrainGenerator.width, 10f * (float) z / terrainGenerator.height);
                Vector3 pos = new Vector3(x, 0, z);
                pos.y = terrainData.SampleHeight(pos);
                
                if (pos.y > seaLevel)
                {
                    Instantiate(sandPrefab, pos,Quaternion.identity);
                }
                if (pos.y > seaLevel && perlinValue1 > 0.6f)
                {
                    Instantiate(treePrefab, pos,Quaternion.identity);
                }
                if (pos.y < seaLevel && perlinValue1 > 0.75)
                {
                    Instantiate(seaweedPrefab, pos,Quaternion.identity);
                }

                if (pos.y < seaLevel && perlinValue1 > 0.4f && perlinValue1 < 0.5f)
                {
                    Debug.Log(perlinValue1);
                    Instantiate(coralPrefab, pos,Quaternion.identity);
                }

            }
        }
    }

    private float CalculateHeightCallback(int x, int y)
    {
        float xCoord = (10f * (float) x / terrainGenerator.width);
        float yCoord = (10f * (float) y / terrainGenerator.height);
        
        float perlinValue2 = Mathf.PerlinNoise(xCoord/frequencyX,yCoord/frequencyY);
        
        return perlinValue2; 
    }
}
    
    /*Vector3 pos = transform.position;
     pos.y = Terrain.activeTerrain.SampleHeight(terrainGenerator.transform.position); 
     
     if (perlinValue > 0.9f)
     {
         float perlinValue2 = Mathf.PerlinNoise(xCoord/frequencyX,yCoord/frequencyY);
         if (perlinValue2 > 0.9f)
         {
             Instantiate(sandPrefab, new Vector3(x,pos.y ,y),Quaternion.identity);
             //Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x,y));
             Debug.Log(pos.y);
         }
     }

     if (perlinValue is > 0.8f and < 0.9f)
     {
         Instantiate(treePrefab, new Vector3(x , pos.y, y),Quaternion.identity);
         //Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x,y));
     }*/
    
        
        /*if (perlinValue > 0.9f)
        {
            float perlinValue2 = Mathf.PerlinNoise(xCoord/frequencyX,yCoord/frequencyY);
            if (perlinValue2 > 0.9f)
            {
                Instantiate(sandPrefab, new Vector3(x,terrainGenerator.terrainDataForRandomExample.GetHeight(x,y) ,y),Quaternion.identity);
                //Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x,y));
            }
        }

        if (perlinValue is > 0.8f and < 0.9f)
        {
           Instantiate(treePrefab, new Vector3(x , terrainGenerator.terrainDataForRandomExample.GetHeight(x,y), y),Quaternion.identity);
           //Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x,y));
        }*/

        /*if (perlinValue > 0.9f)
        {
            //Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x, y));
        }
        return perlinValue;*/

        /*void SpawnSandObject()
        {
            for (int x = 0; x < terrainGenerator.width; x++)
            {
                Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x, 0));
                /*for (int y = 0; y < terrainGenerator.height; y++)
                {
                    Vector3 pos = terrainGenerator.transform.position;
                    pos.y = terrainGenerator.terrainDataForRandomExample.GetHeight(x, y);
                    Debug.Log(terrainGenerator.terrainDataForRandomExample.GetHeight(x, y));
                    //Instantiate(sandPrefab, new Vector3(x, pos.y, y), Quaternion.identity);
                }#1#
            }
    
        }*/


    /*for (int i = 0; i < terrainGenerator.width; i++)
    {
        for (int j = 0; j < terrainGenerator.height; j++)
        {
            Instantiate(sandPrefab);
            sandPrefab.transform.position = new Vector3(i, 0, j);
        }
    }*/

    /*void SpawnSand(float perlinValue)
    {
        for (int i = 0; i < terrainGenerator.width; i++)
        {
            for (int j = 0; j < terrainGenerator.height; j++)
                if (perlinValue > 0.90f)
                {
                    Instantiate(sandPrefab);
                    sandPrefab.transform.position = new Vector3(i, 0, j); 

                }
        }
    }*/

