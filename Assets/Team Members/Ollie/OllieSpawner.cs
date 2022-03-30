using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OllieSpawner : MonoBehaviour
{
    public OllieTerrain ollieTerrain;
    public Terrain myTerrain;
    public GameObject cube;

    [Header("Array")]
    [SerializeField] public float[,] heightsArray;

    private List<Vector3> positions;

    private void Start()
    {
        heightsArray = ollieTerrain.ollieTerrainData.GetHeights(0,0,(int)256,(int)256);
        print(heightsArray.Length);

        /*foreach (var height in heightsArray)
        {
            if (height >= 0.95f)
            {
                
                Vector3 pos = transform.position;
                pos.y = Terrain.activeTerrain.SampleHeight(pos) + Terrain.activeTerrain.GetPosition().y;
                pos.y += 10f;
                GameObject go = Instantiate(cube);
                go.transform.position = pos;
                print("cube pos is "+ pos);
            }
        }*/
        


        /*foreach (var height in heightsArray)
        {
            if (height >= 0.95f)
            {
                print("0.95 height check reached");
                // GameObject go = Instantiate(cube);
                // //go.transform.position = height;
                // Vector3 pos = transform.position;
                // pos.y = Terrain.activeTerrain.SampleHeight(transform.position);
                // go.transform.position = pos;
            }
        }*/

        // for (int x = 0; x < heightsArray.Length/heightsArray.Length; x++)
        // {
        //     for (int z = 0; z < heightsArray.Length / heightsArray.Length; z++)
        //     {
        //         positions.Add(x,heightsArray[i],z);
        //     }
        // }

        for (int x = 0; x < 256; x++)
        {
            for (int z = 0; z < 256; z++)
            {
                Vector3 pos = new Vector3(x,0,z);
                pos.y = myTerrain.SampleHeight(pos);
                if (pos.y > 18f)
                {
                    print("0.95 height check reached");
                    GameObject go = Instantiate(cube);
                    go.transform.position = pos;
                }
                
                // var height = myTerrain.SampleHeight(new Vector3(x, 0, z));
                // if (height >= 10.5f)
                // {
                //     
                //     
                //     
                //     // Vector3 pos = transform.position;
                //     // pos.y = Terrain.activeTerrain.SampleHeight(transform.position);
                //     // go.transform.position = pos;
                // }
            }
        }
    }
}
