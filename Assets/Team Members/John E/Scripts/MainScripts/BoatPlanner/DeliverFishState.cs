using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class DeliverFishState : AntAIState
{
	private Transform t;
	BoatControl boatControl;

	public override void Create(GameObject aGameObject)
	{
		t = aGameObject.GetComponent<Transform>();
		boatControl = aGameObject.GetComponent<BoatControl>();
	}

    public override void Enter()
    {
		StartCoroutine(UnloadFishCoroutine());
    }

	IEnumerator UnloadFishCoroutine()
    {
		yield return new WaitForSeconds(1);

		//Deliver all fish caught with a delay
		if(boatControl.totalFishCaught.Count >= 1)
        {
			do
			{
				boatControl.totalFishCaught.RemoveAt(0);

				yield return new WaitForSeconds(0.5f);
			}
			while (boatControl.totalFishCaught.Count >= 1);
		}

		//Finish when all fish is delivered
		if(boatControl.totalFishCaught.Count <= 0)
        {
			boatControl.HasFish = false;
			Finish();
        }
    }
}
