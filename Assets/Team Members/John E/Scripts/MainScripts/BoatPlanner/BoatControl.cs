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

	public float boatSpeed = 10f;
	public float maxBoatSpeed = 3f;
	public bool useRBForces = false;

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
		return (AntMath.Distance(new Vector2(t.position.x, t.position.z), new Vector2(fishingDock.position.x, fishingDock.position.z)) < 3.5f);
	}
}
