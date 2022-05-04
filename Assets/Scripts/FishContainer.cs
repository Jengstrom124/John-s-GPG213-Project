using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishContainer : MonoBehaviour
{
	public Stack<FishBase> ContainerContents = new Stack<FishBase>();
    
    public int totalFoodAmount;

    public void AddToStomach(FishBase thingToAdd)
    {
	    ContainerContents.Push(thingToAdd);
	    thingToAdd.gameObject.SetActive(false);

	    totalFoodAmount += thingToAdd.GetComponent<IEdible>().GetInfo().amount;
    }

    //Re-Activate all fish in the belly of the shark if it's killed/leaves the game
    public void ReenableEatenFish()
    {
        foreach (var fish in ContainerContents)
        {
	        PopFishFromGuts(fish.GetComponent<IEdible>().GetInfo().amount);
        }
    }

    //Re-activate the last fish eaten if boost used; remove it from the list
    public void PopFishFromGuts(int foodValueToPop)
    {
	    FishBase lastFish = ContainerContents.Peek();
	    
	    totalFoodAmount -= lastFish.GetComponent<IEdible>().GetInfo().amount;
	    
	    //gets the shit spot of the shark
        Vector3 shitSpot = GetComponent<IPredator>().GetBumPosition();
        
        //this SHOULD peek at the last entries position, right?
        lastFish.gameObject.transform.position = shitSpot;

        //reactivate and remove from list
        lastFish.gameObject.SetActive(true);
        
        ContainerContents.Pop();
    }
}
