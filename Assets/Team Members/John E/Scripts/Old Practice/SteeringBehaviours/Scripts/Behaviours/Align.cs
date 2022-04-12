using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBase
{
    Rigidbody rb;
	Neighbours neighbours;
	FishModel fish;

	public float force = 2f;
	
	//Player Fish Stuff
	public bool usePlayerForce = false;
	List<GameObject> currentPlayerNeighbours = new List<GameObject>();
	GameObject currentPlayerFish;

    private void Awake()
    {
		neighbours = GetComponent<Neighbours>();
		fish = GetComponent<FishModel>();
		rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
		fish.onFishChangeEvent += ResetPlayerInfluences;
		fish.onPlayerFishEvent += FollowPlayer;
		
		neighbours.newNeighbourEvent += UpdatePlayerInfluence;
		neighbours.neighbourLeaveEvent += RemovePlayerInfluence;
	}

    private void OnDisable()
    {
		fish.onPlayerFishEvent -= FollowPlayer;
		fish.onFishChangeEvent -= ResetPlayerInfluences;

		neighbours.newNeighbourEvent -= UpdatePlayerInfluence;
		neighbours.neighbourLeaveEvent -= RemovePlayerInfluence;
	}

    void FixedUpdate()
	{
		//rb.AddTorque(CalculateMove(neighbours.neighbours) * force);
		if(!usePlayerForce)
        {
			rb.AddTorque(Vector3.Cross(transform.forward, CalculateMove(neighbours.neighboursList)) * force);
        }
		else
        {
			if(currentPlayerFish != null)
				rb.AddTorque(currentPlayerFish.transform.forward * force);
		}
	}

	public override Vector3 CalculateMove(List<GameObject> neighbours)
	{
		if (neighbours.Count == 0)
        {
			return transform.forward;
        }

		Vector3 alignmentMove = Vector3.zero;

		// Average of all neighbours directions
		foreach (GameObject neighbour in neighbours)
		{
			alignmentMove += neighbour.transform.forward;
		}

		alignmentMove /= neighbours.Count;

		return alignmentMove;
	}

	void FollowPlayer(GameObject playerFish)
    {
		currentPlayerFish = playerFish;

		Neighbours playerFishNeighbours = playerFish.GetComponent<Neighbours>();
		foreach(GameObject playerNeighbour in playerFishNeighbours.neighboursList)
        {
			//Only update influences if not already done
			if (!currentPlayerNeighbours.Contains(playerNeighbour))
            {
				//For Debugging
				playerNeighbour.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = true;

				//Keep track of neighbours
				currentPlayerNeighbours.Add(playerNeighbour);

				//Update align influences
				Align playerNeighbourAlign = playerNeighbour.GetComponent<Align>();
				if (!playerNeighbourAlign.usePlayerForce)
				{
					playerNeighbourAlign.usePlayerForce = true;
				}
            }
			else
            {
				continue;
            }
		}
    }

	void UpdatePlayerInfluence(GameObject other)
    {
		if(other.GetComponent<FishModel>() != null)
        {
			Align align = other.GetComponent<Align>();
			if(align.currentPlayerFish != null)
			{
				align.FollowPlayer(currentPlayerFish);
			}
        }	
    }

	void ResetPlayerInfluences()
    {
		currentPlayerFish = null;
		usePlayerForce = false;

		foreach(GameObject neighbourFish in currentPlayerNeighbours)
		{
			neighbourFish.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = false;
		}

		currentPlayerNeighbours.Clear();
	}

	void RemovePlayerInfluence(GameObject other)
	{
		if(other.GetComponent<FishModel>() != null)
        {
			if(currentPlayerNeighbours.Contains(other))
            {
				other.GetComponentInChildren<Fish_ViewModel>().neighbourDebugColour = false;
				other.GetComponent<Align>().usePlayerForce = false;
				currentPlayerNeighbours.Remove(other);
			}
        }
	}
}
