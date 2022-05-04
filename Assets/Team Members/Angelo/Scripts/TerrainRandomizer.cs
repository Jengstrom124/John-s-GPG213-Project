using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AnGelloStuff
{
    public class TerrainRandomizer : MonoBehaviour
    {
        public int width = 256;
        public int height = 256;

        public float depth = 20;
        public float scale = 50;

        public float offsetX = 100;
        public float offsetY = 100;

        public float testvalue;
        public float testheight;

        public float testcondition;
        public GameObject testobject;

        private void Awake()
        {
            Terrain terrain = GetComponent<Terrain>();
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
            
        }
        void Start()
        {

        }


        void Update()
        {
            //Terrain terrain = GetComponent<Terrain>();
            //terrain.terrainData = GenerateTerrain(terrain.terrainData);
        }

        TerrainData GenerateTerrain(TerrainData terrainData)
        {
            terrainData.heightmapResolution = width + 1;
            terrainData.size = new Vector3(width, depth, height);

            terrainData.SetHeights(0, 0, GenerateHeight());
            return terrainData;
        }

        float[,] GenerateHeight()
        {
            float[,] heights = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(x >= 0 & x <= 10)
                    {
                        heights[x, y] = 10f;
                    }
                    else if (x >= width - 10 & x <= width)
                    {
                        heights[x, y] = 10f;
                    }
                    else if(y >= 0 & y <= 10)
                    {
                        heights[x, y] = 10f;
                    }
                    else if (y >= height - 10 & y <= height)
                    {
                        heights[x, y] = 10f;
                    }
                    else
                    {
                        heights[x, y] = CalculateHeight(x, y);
                    }
                    
                }
            }

            return heights;
        }

        float CalculateHeight(int x, int y)
        {
            float XCoord = (float)x / width * scale + offsetX;
            float YCoord = (float)y / height * scale + offsetY;

            float PerlinData = Mathf.PerlinNoise(XCoord, YCoord);
            if (PerlinData >= testheight)
            {
                return PerlinData * testvalue;
            }
            else
            {
                return 0.5f;
            } 
        }
    }
}

