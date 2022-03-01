using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidModel : MonoBehaviour
{
    SBManager manager;

    public GameObject feeler;
    List<GameObject> myFeelers = new List<GameObject>();

    public int totalFeelers = 3;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SBManager>();

        for(int i = 0; i < totalFeelers; i++)
        {                        
            //spawn a feeler
            GameObject newFeeler = Instantiate(feeler, this.transform);

            //first spawn
            if(i == 0)
            {

            }

            //even
            if (i % 2 == 0)
            {

            }

            //odd numbers
            if (i % 2 == 0)
            {

            }

            //set angle

            //set corresponding ray direction

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
