using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishContainer : MonoBehaviour
{
    public List<IEdible> ContainerContents = new List<IEdible>();

    public void AddToStomach(IEdible thingToAdd)
    {
        ContainerContents.Add(thingToAdd);
    }
}
