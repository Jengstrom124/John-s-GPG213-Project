using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_ViewModel : MonoBehaviour
{
    FishModel boidModel;

    public Material defaultMaterial;

    [Header("Reference Only:")]
    bool isPlayerFish;
    public bool neighbourDebugColour = false;

    // Start is called before the first frame update
    void Start()
    {
        boidModel = GetComponentInParent<FishModel>();
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerFish = boidModel.isPlayerFish;

        if (isPlayerFish)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        }
        else
        {
            if (!neighbourDebugColour)
            {
                gameObject.GetComponent<Renderer>().material = defaultMaterial;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }
}
