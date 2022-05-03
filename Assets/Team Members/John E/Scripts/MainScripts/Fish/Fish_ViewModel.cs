using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_ViewModel : MonoBehaviour
{
    FishModel fish;
    public Material defaultMaterial;

    [Header("Reference Only:")]
    public bool neighbourDebugColour = false;

    // Start is called before the first frame update
    void Awake()
    {
        fish = GetComponentInParent<FishModel>();
    }

    private void Start()
    {
        fish.onPlayerFishEvent += UpdateColour;
    }

    void UpdateColour(bool isPlayerFish)
    {
        if(isPlayerFish)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = defaultMaterial;
        }
    }
}
