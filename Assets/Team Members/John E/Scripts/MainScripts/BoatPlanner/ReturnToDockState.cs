using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class ReturnToDockState : AntAIState
{
	private Transform t;
	Transform fishingDock;
	Rigidbody rb;

	BoatControl boatControl;
	PathTracker pathTracker;

	public override void Create(GameObject aGameObject)
	{
		t = aGameObject.GetComponent<Transform>();
		boatControl = aGameObject.GetComponent<BoatControl>();
		rb = aGameObject.GetComponent<Rigidbody>();
		pathTracker = aGameObject.GetComponent<PathTracker>();
	}

	public override void Enter()
	{
		//fishingDock = GameObject.Find("FishingDock");
		fishingDock = boatControl.GetDock();

		if(fishingDock == null)
        {
			Debug.Log("Dock not found!");
			Finish();
		}
		else
        {
			Vector3 fishingDockPos = fishingDock.position;

			if (AStar.Instance != null)
			{
				AStar.Instance.FindPath(t, fishingDockPos);
			}
		}
	}

	public override void Execute(float aDeltaTime, float aTimeScale)
	{
		//Hard Moving Transform - Optional to use this or rb forces
		if (pathTracker.currentTargetPos != Vector2.zero && !boatControl.useRBForces)
		{
			Vector3 targetPos;
			targetPos = new Vector3(pathTracker.currentTargetPos.x, t.position.y, pathTracker.currentTargetPos.y);
			t.position = Vector3.MoveTowards(t.position, targetPos, boatControl.boatSpeed * aDeltaTime);
		}

		// Check distance to Dock
		if (boatControl.AtDock())
		{
			rb.velocity = Vector3.zero;
			//pathTracker.ResetPathTracking();
			Finish();
		}
		else
        {
			//Keep moving to dock (using rb forces)
			if (boatControl.useRBForces)
				rb.AddRelativeForce(Vector3.forward * boatControl.boatSpeed * aDeltaTime);
		}
	}
}
