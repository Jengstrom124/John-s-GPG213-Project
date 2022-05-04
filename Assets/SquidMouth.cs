using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidMouth : MonoBehaviour
{
    public IPredator iPredator;
    public mayaSquid squid;
    
    public void OnTriggerEnter(Collider other)
    {
        IEdible edible = other.GetComponent<IEdible>();

        if (edible != null)
        {
            edible.GetEaten(iPredator);

            if (edible.GetInfo().edibleType == EdibleType.Food)
            {
                squid.canInk = true;
            }
            // more ifs for poison and booster etc
        }
    }
}
