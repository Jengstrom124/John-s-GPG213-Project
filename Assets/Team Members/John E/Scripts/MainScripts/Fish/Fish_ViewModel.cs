using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_ViewModel : MonoBehaviour
{
    FishModel fish;
    public Material defaultMaterial;

    [Header("Reference Only:")]
    public bool neighbourDebugColour = false;
    Color defaultColor;

    // Start is called before the first frame update
    void Awake()
    {
        fish = GetComponentInParent<FishModel>();
    }

    private void Start()
    {
        fish.onPlayerFishEvent += UpdateColour;
        defaultColor = GetComponentInChildren<Renderer>().material.color;
    }

    void UpdateColour(bool isPlayerFish)
    {
        if(isPlayerFish)
        {
            //gameObject.GetComponent<Renderer>().material.color = Color.cyan;
            GetComponentInChildren<Renderer>().material.color = Color.cyan;
        }
        else
        {
            //gameObject.GetComponent<Renderer>().material = defaultMaterial;
            GetComponentInChildren<Renderer>().material.color = defaultColor;
        }
    }
}
