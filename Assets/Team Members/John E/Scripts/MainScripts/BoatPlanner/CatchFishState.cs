using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class CatchFishState : AntAIState
{
	private Transform t;
	BoatControl boatControl;

	[Tooltip("Any fish within the defined radius will be caught")]
	public float fishCatchingRadius = 5f;
	public LayerMask fishLayerMask;

	public override void Create(GameObject aGameObject)
	{
		t = aGameObject.GetComponent<Transform>();
		boatControl = aGameObject.GetComponent<BoatControl>();
	}

	public override void Enter()
	{
		StartCoroutine(CatchFishCoroutine());
	}

	IEnumerator CatchFishCoroutine()
    {
		yield return new WaitForSeconds(2);

		//if fish is within catch radius - catch it
		if (Physics.CheckSphere(t.position, fishCatchingRadius, fishLayerMask.value, QueryTriggerInteraction.Ignore))
		{
			foreach(Collider fishCollider in Physics.OverlapSphere(t.position, fishCatchingRadius, fishLayerMask.value, QueryTriggerInteraction.Ignore))
            {
				boatControl.totalFishCaught.Add(fishCollider.gameObject);
            }

			boatControl.HasFish = true;
			boatControl.AtFishingSpot = false;
			Finish();

		}
		else
		{
			Debug.Log("No fish!");
			boatControl.AtFishingSpot = false;
			Finish();
		}
	}
}
