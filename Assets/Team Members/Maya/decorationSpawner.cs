using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decorationSpawner : MonoBehaviour
{
    public Terrain myTerrain;
    private Vector3 bigBoy;

    public List<GameObject> landStuff;
    public List<GameObject> underwaterStuff;
    // Start is called before the first frame update
    void Start()
    {
bigBoy = new Vector3(2, 2, 2);

        for (int x = 0; x < 256; x++)
        {
            for (int z = 0; z < 256; z++)
            {
                Vector3 pos = new Vector3(x,0,z);
                pos.y = myTerrain.SampleHeight(pos);
                if (pos.y > 12f && pos.y < 18)
                {
                    if (UnityEngine.Random.Range(0, 100) > 95)
                    {
                        int prefabIndex = Random.Range(0, landStuff.Capacity);
                        GameObject go = Instantiate(landStuff[prefabIndex]);
                        go.transform.position = pos;
                        go.transform.localScale = bigBoy;
                        go.transform.parent = this.transform;
                    }
                }
                else if (pos.y < 9f)
                {
                    if (UnityEngine.Random.Range(0, 100) > 97.5)
                    {
                        int prefabIndex = Random.Range(0, underwaterStuff.Capacity);
                        GameObject go = Instantiate(underwaterStuff[prefabIndex]);
                        go.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        go.transform.localScale = bigBoy;
                        go.transform.parent = this.transform;
                    }
                }
            }
        }
    }
}

    // Update is called once per frame

