using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kevin
{
    public class MiscSpawner : MonoBehaviour
    {
        public TerrainGenerator terrainGenerator;
        public Terrain terrainData;
        
        public List<GameObject> gameobjectParents;
        public GameObject sandPrefab;
        public GameObject seaweedPrefab;
        public GameObject coralPrefab;
        public List<GameObject> treePrefab;
        
        public float seaLevel = 15f;
        public float seaweedLevel = 12f;
        public float coralLevel = 7f;
        
        public bool sand;
        public bool tree;
        public bool seaweed;
        public bool coral;
        public void Start()
        { 
            LandSpawner();
        }

        public void LandSpawner()
        {
            //yield return new WaitForSeconds(2f);
            for (int x = 0; x < terrainGenerator.width; x++)
            {
                for (int z = 0; z < terrainGenerator.height; z++)
                {
                    float perlinValue1 = Mathf.PerlinNoise(10f * (float) x / terrainGenerator.width, 10f * (float) z / terrainGenerator.height);
                    Vector3 pos = new Vector3(x, 0, z);
                    pos.y = terrainData.SampleHeight(pos);
                    GameObject parent = gameobjectParents[0];
                    GameObject parent1 = gameobjectParents[1];
                    GameObject parent2 = gameobjectParents[2];
                    if (pos.y > seaLevel && sand)
                    {
                        Instantiate(sandPrefab, pos,Quaternion.identity);
                    }
                    if (pos.y > seaLevel && perlinValue1 > 0.6f && tree)
                    {
                        int randomTree = Random.Range(0, treePrefab.Count);
                        Instantiate(treePrefab[randomTree], pos,Quaternion.identity,parent.transform);
                    }
                    if (pos.y < seaweedLevel && pos.y > coralLevel && perlinValue1 > 0.75 && seaweed)
                    {
                        Instantiate(seaweedPrefab, pos,Quaternion.identity,parent1.transform);
                    }

                    if (pos.y < coralLevel && perlinValue1 > 0.475f && perlinValue1 < 0.5f && coral)
                    {
                        Instantiate(coralPrefab, pos,Quaternion.identity,parent2.transform);
                    }

                }
            }
        }

    }
}

