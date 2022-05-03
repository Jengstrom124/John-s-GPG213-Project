using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class SearchForFishingSpotState : AntAIState
{
	private Transform t;
	Rigidbody rb;
	GameObject targetFishGO;
	bool fishAssigned;
	bool resetThresholdMet = false;

	BoatControl boatControl;
	PathTracker pathTracker;

	FishModel[] potentialFish;
	Vector3 fishPos;

	public override void Create(GameObject aGameObject)
	{
		t = aGameObject.GetComponent<Transform>();
		boatControl = aGameObject.GetComponent<BoatControl>();
		rb = aGameObject.GetComponent<Rigidbody>();
		pathTracker = aGameObject.GetComponent<PathTracker>();
	}

	public override void Enter()
	{
		//As this is run multiple times - always reset fish variable upon entering this state to prevent boat travelling to wrong fish (Keep fish up to date)
		targetFishGO = null;
		resetThresholdMet = false;

		//Using a coroutine to find fish as it seems to try to find a fish before they spawn causing null errors
		StartCoroutine(FindFish());
	}

	public override void Execute(float aDeltaTime, float aTimeScale)
    {
		if(targetFishGO != null && targetFishGO.activeSelf == true)
        {
			if(pathTracker.currentTargetPos != Vector2.zero && !boatControl.useRBForces)
            {
				Vector3 targetPos;
				targetPos = new Vector3(pathTracker.currentTargetPos.x, t.position.y, pathTracker.currentTargetPos.y);
				t.position = Vector3.MoveTowards(t.position, targetPos, boatControl.boatSpeed * aDeltaTime);
            }

			// Check distance to fish
			if (AntMath.Distance(new Vector2(t.position.x, t.position.z), new Vector2(fishPos.x, fishPos.z)) <= boatControl.distanceThreshold)
			{
				// We arrived!
				// Current action is finished.
				boatControl.AtFishingSpot = true;
				rb.velocity = Vector3.zero;
				Finish();
			}
			else
            {
				if(boatControl.useRBForces)
					rb.AddRelativeForce(Vector3.forward * boatControl.boatSpeed * aDeltaTime);
			}

			//For resetting state when bugged
			if (resetThresholdMet && pathTracker.currentTargetPos == Vector2.zero)
			{
				if(resetThresholdMet)
                {
					Debug.Log("Resetting... Changing drivers");
					Finish();
					resetThresholdMet = false;
				}
			}
		}
		else
        {
			if (fishAssigned)
			{
				Debug.Log("Find new fish!");
				pathTracker.ResetPathTracking();
				Finish();
				fishAssigned = false;
			}
		}
	}

	IEnumerator FindFish()
    {
		fishAssigned = false;

		yield return new WaitForSeconds(1f);

		if (FindObjectOfType<FishModel>() == null)
		{
			Debug.Log("no fish found!");
			Finish();
			StopAllCoroutines();
		}
		else
        {
			//HACK: Using this to find fish for now
			potentialFish = FindObjectsOfType<FishModel>();
			FishModel fishTarget = potentialFish[0];

			//Find fish with most neighbours to choose as fishing spot
			for(int i = 0; i < potentialFish.Length; i++)
            {
				if (potentialFish[i].GetComponent<Neighbours>().neighboursList.Count > fishTarget.GetComponent<Neighbours>().neighboursList.Count)
				{
					fishTarget = potentialFish[i];
				}
			}

			targetFishGO = fishTarget.gameObject;
			fishPos = targetFishGO.transform.position;
			fishAssigned = true;

			if (AStar.Instance != null)
			{
				AStar.Instance.FindPath(t, fishPos);
			}
		}

		yield return new WaitForSeconds(10f);

		resetThresholdMet = true;
	}

	//OLD - ignore this
	IEnumerator GoToFishCoroutine()
    {
		yield return new WaitForSeconds(0.5f);

		targetFishGO = FindObjectOfType<FishModel>().gameObject;

		if (targetFishGO != null)
		{
			Vector3 pos = t.position;

			pos.x = targetFishGO.transform.position.x / 2;
			pos.z = targetFishGO.transform.position.z / 2;
			t.position = pos;

			yield return new WaitForSeconds(1);

			pos.x = targetFishGO.transform.position.x;
			pos.z = targetFishGO.transform.position.z;
			t.position = pos;

			yield return new WaitForSeconds(0.25f);

			boatControl.AtFishingSpot = true;
			Finish();
		}
		else
		{
			Debug.Log("fish not found!");
			Finish();
		}
	}
}
