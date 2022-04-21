using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class SearchForFishingSpotState : AntAIState
{
	private Transform t;
	BoatControl boatControl;
	GameObject fish;

	public override void Create(GameObject aGameObject)
	{
		t = aGameObject.GetComponent<Transform>();
		boatControl = aGameObject.GetComponent<BoatControl>();
	}

	public override void Enter()
	{
		//StartCoroutine(GoToFishCoroutine());

		fish = FindObjectOfType<FishModel>().gameObject;

		if (fish == null)
		{
			Debug.Log("fish not found!");
			Finish();
		}
	}

	public override void Execute(float aDeltaTime, float aTimeScale)
    {
		Vector3 fishPos = fish.transform.position;

		t.position = Vector3.MoveTowards(t.position, new Vector3(fishPos.x, t.position.y, fishPos.z), boatControl.boatSpeed * aDeltaTime);

		// Check distance to fish
		if (AntMath.Distance(new Vector2(t.position.x, t.position.z), new Vector2(fishPos.x, fishPos.z)) <= 2f)
		{
			// We arrived!
			// Current action is finished.
			boatControl.AtFishingSpot = true;
			Finish();
		}
	}

	IEnumerator GoToFishCoroutine()
    {
		yield return new WaitForSeconds(0.5f);

		fish = FindObjectOfType<FishModel>().gameObject;

		if (fish != null)
		{
			Vector3 pos = t.position;

			pos.x = fish.transform.position.x / 2;
			pos.z = fish.transform.position.z / 2;
			t.position = pos;

			yield return new WaitForSeconds(1);

			pos.x = fish.transform.position.x;
			pos.z = fish.transform.position.z;
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
