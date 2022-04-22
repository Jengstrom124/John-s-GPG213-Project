using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class ReturnToDockState : AntAIState
{
	private Transform t;
	BoatControl boatControl;
	GameObject fishingDock;

	public override void Create(GameObject aGameObject)
	{
		t = aGameObject.GetComponent<Transform>();
		boatControl = aGameObject.GetComponent<BoatControl>();
	}

	public override void Enter()
	{
		//StartCoroutine(ReturnToDockCoroutine());

		fishingDock = GameObject.Find("FishingDock");

		if(fishingDock == null)
        {
			Debug.Log("Dock not found!");
			Finish();
		}
	}

	public override void Execute(float aDeltaTime, float aTimeScale)
	{
		Vector3 fishingDockPos = fishingDock.transform.position;

		t.position = Vector3.MoveTowards(t.position, new Vector3(fishingDockPos.x, t.position.y, fishingDockPos.z), boatControl.boatSpeed * aDeltaTime);

		// Check distance to Dock
		if (boatControl.AtDock())
		{
			Finish();
		}
	}

	IEnumerator ReturnToDockCoroutine()
	{
		yield return new WaitForSeconds(1);

		fishingDock = GameObject.Find("FishingDock");

		if (fishingDock != null)
		{
			Vector3 pos = t.position;

			pos.x = fishingDock.transform.position.x / 2;
			pos.z = fishingDock.transform.position.z / 2;
			t.position = pos;

			yield return new WaitForSeconds(1);

			pos.x = fishingDock.transform.position.x;
			pos.z = fishingDock.transform.position.z;
			t.position = pos;

			yield return new WaitForSeconds(0.1f);

			Finish();
		}
		else
		{
			Debug.Log("Dock not found!");
			Finish();
		}
	}
}
