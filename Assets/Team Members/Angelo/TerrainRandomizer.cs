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
        void Start()
        {

        }


        void Update()
        {
            Terrain terrain = GetComponent<Terrain>();
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
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
                    heights[x, y] = CalculateHeight(x, y);
                }
            }

            return heights;
        }

        float CalculateHeight(int x, int y)
        {
            float XCoord = (float)x / width * scale + offsetX;
            float YCoord = (float)y / height * scale + offsetY;

            return Mathf.PerlinNoise(XCoord, YCoord);
        }
    }
}

