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
		StartCoroutine(GoToFishCoroutine());
	}

	IEnumerator GoToFishCoroutine()
    {
		yield return new WaitForSeconds(1);

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
