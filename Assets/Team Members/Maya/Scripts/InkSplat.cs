using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InkSplat : MonoBehaviour
{
    public GameObject inkView;

    public List<Material> inks;
    // Start is called before the first frame update
    void Start()
    {
        int chosenOne = Random.Range(0, inks.Capacity);
        inkView.GetComponent<MeshRenderer>().material = inks[chosenOne];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        IReactsToInk inkable = other.GetComponent<IReactsToInk>();

        if (inkable != null)
        {
            inkable.ChangeInkedState(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        IReactsToInk inkable = other.GetComponent<IReactsToInk>();

        if (inkable != null)
        {
            inkable.ChangeInkedState(false);
        }
    }
}
