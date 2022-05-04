using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishContainer : MonoBehaviour
{
	// Change to Stack, use Push/Pop
    public List<FishBase> ContainerContents = new List<FishBase>();
    
    public void AddToStomach(FishBase thingToAdd)
    {
        ContainerContents.Add(thingToAdd);
        thingToAdd.enabled = false;// TODO .gameObject.SetActive(true)
    }

    //Re-Activate all fish in the belly of the shark if it's killed/leaves the game
    public void ReenableEatenFish()
    {
        foreach (var fish in ContainerContents)
        {
	        // Call Popfish
        }
        
        ContainerContents.Clear();
    }

    //Re-activate the last fish eaten if boost used; remove it from the list
    public void PopFishFromGuts(FishBase lastFish) // TODO change sig to int foodValueToPop
    {
	    // TODO: Stack you can remove most of this
        //finds last fish
        int lastIndex = ContainerContents.Count - 1;
        lastFish = ContainerContents[lastIndex];
        
        //gets the shit spot of the shark
        Vector3 shitSpot = GetComponent<IPredator>().GetBumPosition();
        lastFish.transform.position = shitSpot; // TODO use .Peek to get lastfish position
        
        //reactivate and remove from list
        lastFish.enabled = true; // TODO .gameObject.SetActive(true)
        ContainerContents.Remove(lastFish);
    }
}
