using Anthill.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoatControl : MonoBehaviour
{
	bool hasFish;
	bool atFishingSpot;

	Transform t;
	Transform fishingDock;

	[Header("Reference Only:")]
	public List<GameObject> totalFishCaught = new List<GameObject>();

	private void Awake()
	{
		t = GetComponent<Transform>();

		// Save reference for fishing dock.
		GameObject fishingDockGO = GameObject.Find("FishingDock");
		Debug.Assert(fishingDockGO != null, "Base object not exists on the scene!");
		fishingDock = fishingDockGO.GetComponent<Transform>();
	}

	public bool HasFish
	{
		get { return hasFish; }
		set
		{
			hasFish = value;
		}
	}

	public bool AtFishingSpot
    {
		get { return atFishingSpot; }
		set
		{
			atFishingSpot = value;
		}
	}

	public bool AtDock()
	{
		return (AntMath.Distance(t.position, fishingDock.position) < 5f);
	}
}
