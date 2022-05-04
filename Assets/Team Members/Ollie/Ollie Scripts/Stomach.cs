using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomach : MonoBehaviour
{
    public IPredator iPredator;
    
    public void OnTriggerEnter(Collider other)
    {
        IEdible edible = other.GetComponent<IEdible>();

        if (edible != null)
        {
            edible.GetEaten(iPredator);

            if (edible.GetInfo().edibleType == EdibleType.Food)
            {
                // increase scale
                // add fish to stomach (container)
                print("nomnomnom");
            }
        }
    }
}
