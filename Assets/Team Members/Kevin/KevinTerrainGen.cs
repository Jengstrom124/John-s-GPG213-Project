using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Tree = Unity.VisualScripting.Antlr3.Runtime.Tree.Tree;

public class KevinTerrainGen : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    public Terrain terrainData;

    public float offsetX;
    public float offsetY;
    
    
    public float fringe = 0.05f;
    public float frequencyX = 1f;
    public float frequencyY = 1f;
    public int yHeight = 20;
    
    public float cliffLevel = 30f;
    public float seaLevel = 10f;
    public float highLands = 0.75f;
    public float landDrop = 0.4f;
    public float control = 0.5f;
    
    public GameObject sandPrefab;
    //public GameObject treePrefab;
    public GameObject seaweedPrefab;
    public GameObject coralPrefab;
    public List<GameObject> treePrefab;
    private List<Vector3> currentPosition;
    
    
    //bools used to test the different prefabs

    public bool sand;
    public bool tree;
    public bool seaweed;
    public bool coral;
    
    void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        
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
                
                if (pos.y > seaLevel && sand)
                {
                    Instantiate(sandPrefab, pos,Quaternion.identity);
                }
                if (pos.y > seaLevel && perlinValue1 > 0.6f && tree)
                {
                    int randomTree = Random.Range(0, treePrefab.Count);
                    Instantiate(treePrefab[randomTree], pos,Quaternion.identity);
                }
                if (pos.y < seaLevel && perlinValue1 > 0.75 && seaweed)
                {
                    Instantiate(seaweedPrefab, pos,Quaternion.identity);
                }

                if (pos.y < seaLevel && perlinValue1 > 0.4f && perlinValue1 < 0.5f && coral)
                {
                    //Debug.Log(perlinValue1);
                    Instantiate(coralPrefab, pos,Quaternion.identity);
                }

            }
        }
    }

    private float CalculateHeightCallback(int x, int y)
    {
        float xCoord = ((float) x / terrainGenerator.width);
        float yCoord = ((float) y / terrainGenerator.height);
        
        
        float perlinValue = Mathf.PerlinNoise(xCoord*frequencyX+offsetX,yCoord*frequencyY+offsetY);
        float perlinHighLand = 0.9f * Mathf.PerlinNoise(xCoord * frequencyX + offsetX, yCoord * frequencyY + offsetY);
        float perlinDropLand = 0.1f * Mathf.PerlinNoise(xCoord * frequencyX + offsetX, yCoord * frequencyY + offsetY);

        if (perlinHighLand > highLands)
        {
            return perlinHighLand + perlinDropLand;
        }

        if (perlinHighLand < landDrop) ;
        {
            return perlinHighLand * 0.75f;
        }
        //return perlinHighLand;
        
        /*if (posTerrain.y < 9)
        {
            float perlinValue1 = Mathf.PerlinNoise(xCoord*frequencyX+offsetX*40f,yCoord*frequencyY+offsetY*40f);
            return perlinValue1 * 0.1f;
        }*/
        /*if (perlinValue > 0.86f && perlinValue < 1f)
        {
            return Mathf.Clamp(perlinValue * 0.9f,0.01f,0.99f);
        }
        if (perlinValue > 0.75f && perlinValue < 0.85f)
        {
            float perlinValue1 = Mathf.PerlinNoise(xCoord*frequencyX+offsetX*40f,yCoord*frequencyY+offsetY*40f);
            return Mathf.Clamp(perlinValue1 * 0.75f,0.01f,0.99f);
        }
        if (perlinValue > 0.61f && perlinValue < 0.74f)
        {
            float perlinValue2 = Mathf.PerlinNoise(xCoord*frequencyX+offsetX*40f,yCoord*frequencyY+offsetY*40f);
            return Mathf.Clamp(perlinValue2 * 0.6f,0.01f,0.99f);
        }
        if (perlinValue > 0.51f && perlinValue < 0.6f)
        {
            float perlinValue3 = Mathf.PerlinNoise(xCoord*frequencyX+offsetX*40f,yCoord*frequencyY+offsetY*40f);
            return Mathf.Clamp(perlinValue3 * 0.5f,0.01f,0.99f);
        }
        if (perlinValue < 0.5f)
        {
            float perlinValue4 = Mathf.PerlinNoise(xCoord*frequencyX+offsetX*40f,yCoord*frequencyY+offsetY*40f);
            return Mathf.Clamp(perlinValue4 * 0.1f,0.01f,0.99f);
        }*/

        
        /*if (!(xCoord < fringe || xCoord > 1 - fringe || yCoord < fringe || yCoord > 1 - fringe))
        {
            return perlinValue; 
        }

        return previousHeights[x, y];*/
        /*if (perlinValue < 0.7f)
        {
            float perlinValue2 = Mathf.PerlinNoise(10f*xCoord*frequencyX+offsetX,10f*yCoord*frequencyY+offsetY);
            return perlinValue2;
        }
        else
        {
            
        }*/
        return perlinValue;
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


