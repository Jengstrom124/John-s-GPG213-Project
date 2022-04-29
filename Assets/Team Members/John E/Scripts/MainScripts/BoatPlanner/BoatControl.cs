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
	FishingDock[] fishingDocks;
	PathTracker pathTracker;

	public float boatSpeed = 10f;
	public float maxBoatSpeed = 3f;
	public bool useRBForces = false;

	[Header("Reference Only:")]
	public List<GameObject> totalFishCaught = new List<GameObject>();
	public float distanceThreshold;

	private void Awake()
	{
		t = GetComponent<Transform>();
		pathTracker = GetComponent<PathTracker>();

		// Save reference for fishing dock.
		fishingDocks = FindObjectsOfType<FishingDock>();
		//GameObject fishingDockGO = GameObject.Find("FishingDock");
		Debug.Assert(fishingDocks != null, "Base object not exists on the scene!");
		//fishingDock = fishingDockGO.GetComponent<Transform>();
		
	}

    private void Start()
    {
		distanceThreshold = pathTracker.distanceThreshold +1;
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
		if(fishingDock == null)
			GetDock();

		return (AntMath.Distance(new Vector2(t.position.x, t.position.z), new Vector2(fishingDock.position.x, fishingDock.position.z)) < distanceThreshold);
	}

	public Transform GetDock()
    {
		fishingDock = fishingDocks[0].transform;

		//Find the closest fishing dock
		for (int i = 1; i < fishingDocks.Length; i++)
		{
			Transform tempFishingDock = fishingDocks[i].transform;
			if (AntMath.Distance(new Vector2(t.position.x, t.position.z), new Vector2(tempFishingDock.position.x, tempFishingDock.position.z)) < AntMath.Distance(new Vector2(t.position.x, t.position.z), new Vector2(fishingDock.position.x, fishingDock.position.z)))
			{
				fishingDock = tempFishingDock;
			}
		}

		return fishingDock;
	}
}
