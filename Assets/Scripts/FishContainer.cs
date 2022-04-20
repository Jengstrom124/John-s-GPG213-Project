using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishContainer : MonoBehaviour
{
    public List<FishBase> ContainerContents = new List<FishBase>();
    
    public void AddToStomach(FishBase thingToAdd)
    {
        ContainerContents.Add(thingToAdd);
        thingToAdd.enabled = false;
    }

    //Re-Activate all fish in the belly of the shark if it's killed/leaves the game
    public void ReenableEatenFish()
    {
        foreach (var fish in ContainerContents)
        {
            fish.enabled = true;
        }
        
        ContainerContents.Clear();
    }

    //Re-activate the last fish eaten if boost used; remove it from the list
    public void PopFishFromGuts(FishBase lastFish)
    {
        //finds last fish
        int lastIndex = ContainerContents.Count - 1;
        lastFish = ContainerContents[lastIndex];
        
        //gets the shit spot of the shark
        Vector3 shitSpot = GetComponent<IPredator>().GetBumPosition();
        lastFish.transform.position = shitSpot;
        
        //reactivate and remove from list
        lastFish.enabled = true;
        ContainerContents.Remove(lastFish);
    }
}
