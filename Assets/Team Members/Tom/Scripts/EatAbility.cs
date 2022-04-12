using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatAbility : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        IEdible edible = other.GetComponent<IEdible>();

        if (edible != null)
        {
            //edible.GetEaten(this);
        }
    }
}
