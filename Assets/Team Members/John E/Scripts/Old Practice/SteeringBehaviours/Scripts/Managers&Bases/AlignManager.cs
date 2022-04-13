using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignManager : MonoBehaviour
{
	Align align;
	List<GameObject> currentPlayerNeighbours = new List<GameObject>();
	GameObject currentPlayerFish;

    private void Awake()
    {
		align = GetComponent<Align>();
    }
    private void OnEnable()
	{
		FishModel.onPlayerFishEvent += UpdatePlayerInfluences;

		Neighbours.newNeighbourEvent += NewFishUpdate;
		Neighbours.neighbourLeaveEvent += RemovePlayerInfluence;
	}
    private void OnDisable()
	{
		FishModel.onPlayerFishEvent -= UpdatePlayerInfluences;

		Neighbours.newNeighbourEvent -= NewFishUpdate;
		Neighbours.neighbourLeaveEvent -= RemovePlayerInfluence;
	}

    private void Update()
    {
        foreach(GameObject fish in currentPlayerNeighbours)
        {
			//For Debugging
			fish.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = true;

			//Update align influences
			if(currentPlayerFish.GetComponent<FishModel>().hasWaypoint)
            {
				Align playerNeighbourAlign = fish.GetComponent<Align>();
				playerNeighbourAlign.currentPlayerFish = currentPlayerFish;
				if (!playerNeighbourAlign.usePlayerForce)
				{
					playerNeighbourAlign.usePlayerForce = true;
				}
			}
		}
    }

	void UpdatePlayerInfluences(GameObject fish)
	{
		if (fish.GetComponent<FishModel>() != null)
		{
			if (currentPlayerFish != null)
			{
				if (currentPlayerNeighbours.Count > 0)
				{
					foreach (GameObject neighbourFish in currentPlayerNeighbours)
					{
						neighbourFish.GetComponent<Align>().usePlayerForce = false;
						neighbourFish.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = false;
					}

					currentPlayerNeighbours.Clear();
				}

				FindAllPlayerNeighbours(fish);
			}
			else
			{
				FindAllPlayerNeighbours(fish);
			}
		}
	}

	void FindAllPlayerNeighbours(GameObject playerFish)
	{
		currentPlayerFish = playerFish;

		Neighbours playerFishNeighbours = playerFish.GetComponentInParent<Neighbours>();
		foreach (GameObject playerNeighbour in playerFishNeighbours.neighboursList)
		{
			//Only update influences if not already done
			if (!currentPlayerNeighbours.Contains(playerNeighbour))
			{
				//AddPlayerInfluences(playerNeighbour);
				currentPlayerNeighbours.Add(playerNeighbour);
			}
		}
	}

	//When a new fish enters players neighbour radius - add player influences to it
	void NewFishUpdate(GameObject other)
	{
		if (other.GetComponent<FishModel>() != null)
		{
			//Align align = other.GetComponentInParent<Align>();
			if(currentPlayerFish != null && currentPlayerFish.GetComponent<Neighbours>().neighboursList.Contains(other))
			{
				//AddPlayerInfluences(other);
				currentPlayerNeighbours.Add(other);
			}
		}
	}

	void RemovePlayerInfluence(GameObject other)
	{
		if (other.GetComponent<FishModel>() != null)
		{
			if (currentPlayerNeighbours.Contains(other))
			{
				currentPlayerNeighbours.Remove(other);
				other.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = false;
				other.GetComponent<Align>().usePlayerForce = false;
				other.GetComponent<Align>().currentPlayerFish = null;
			}
		}
	}

	/*
	void AddPlayerInfluences(GameObject fishToAddInfluenceTo)
    {
		//For Debugging
		fishToAddInfluenceTo.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = true;

		//Keep track of neighbours
		currentPlayerNeighbours.Add(fishToAddInfluenceTo);

		//Update Influences
		Align playerNeighbourAlign = fishToAddInfluenceTo.GetComponent<Align>();
		playerNeighbourAlign.currentPlayerFish = currentPlayerFish;
		if (!playerNeighbourAlign.usePlayerForce)
		{
			playerNeighbourAlign.usePlayerForce = true;
		}
	}
	*/
}
