using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class BoatSense : MonoBehaviour, ISense
{
	private BoatControl boatControl;

    private void Awake()
    {
        boatControl = GetComponent<BoatControl>();
    }

    public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
    {
        aWorldState.Set(FishingBoat.hasFish, boatControl.HasFish);
        aWorldState.Set(FishingBoat.atFishingSpot, boatControl.AtFishingSpot);
        aWorldState.Set(FishingBoat.atDock, boatControl.AtDock());
        aWorldState.Set(FishingBoat.fishDelivered, false);
    }
}
