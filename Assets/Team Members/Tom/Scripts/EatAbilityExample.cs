using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatAbilityExample : MonoBehaviour
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
                // I got food
                // MAKE BIG, ADD FISH TO STOMACH
            }
            // more ifs for poison and booster etc
        }
    }
}
