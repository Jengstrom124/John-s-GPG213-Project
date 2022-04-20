using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OllieSpawner : MonoBehaviour
{
    //public OllieTerrain ollieTerrain;
    public Terrain myTerrain;
    //public GameObject cube;
    private Vector3 scaleUp;
    
    //public list, drag desired prefabs into it
    public List<GameObject> treePrefabList;
    public List<GameObject> shorelinePrefabList;
    public List<GameObject> underwaterDebrisList;

    // [Header("Array")]
    // [SerializeField] public float[,] heightsArray;

    private void Start()
    {
        //heightsArray = ollieTerrain.ollieTerrainData.GetHeights(0,0,(int)256,(int)256);
        scaleUp = new Vector3(2, 2, 2);

        for (int x = 0; x < 256; x++)
        {
            for (int z = 0; z < 256; z++)
            {
                Vector3 pos = new Vector3(x,0,z);
                pos.y = myTerrain.SampleHeight(pos);
                if (pos.y > 15f && pos.y < 18)
                {
                    if (UnityEngine.Random.Range(0, 100) > 95)
                    {
                        //basically - 25% chance to spawn a random tree, at every point above y=15
                        int prefabIndex = UnityEngine.Random.Range(0, treePrefabList.Capacity);
                        GameObject go = Instantiate(treePrefabList[prefabIndex]);
                        go.transform.position = pos;
                        go.transform.localScale = scaleUp;
                        go.transform.parent = this.transform;
                    }
                }
                else if (pos.y < 15f && pos.y > 12f)
                {
                    if (UnityEngine.Random.Range(0, 100) > 95)
                    {
                        //basically - 25% chance to spawn a random tree, at every point above y=15
                        int prefabIndex = UnityEngine.Random.Range(0, shorelinePrefabList.Capacity);
                        GameObject go = Instantiate(shorelinePrefabList[prefabIndex]);
                        go.transform.position = pos;
                        go.transform.localScale = scaleUp;
                        go.transform.parent = this.transform;
                    }
                }
                else if (pos.y < 9f)
                {
                    //spawn bushes/rocks/shells/debris?
                    if (UnityEngine.Random.Range(0, 100) > 97.5)
                    {
                        int prefabIndex = UnityEngine.Random.Range(0, underwaterDebrisList.Capacity);
                        GameObject go = Instantiate(underwaterDebrisList[prefabIndex]);
                        go.transform.position = new Vector3(pos.x, 0.05f, pos.z);
                        go.transform.localScale = scaleUp;
                        go.transform.parent = this.transform;
                    }
                }
            }
        }
    }
}
