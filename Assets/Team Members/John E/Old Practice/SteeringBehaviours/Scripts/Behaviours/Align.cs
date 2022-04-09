using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBase
{
    Rigidbody rb;
	Neighbours neighbours;
	BoidModel fish;

	public float force = 2f;
	bool usePlayerForce = false;

    private void Start()
    {
		neighbours = GetComponent<Neighbours>();
		fish = GetComponent<BoidModel>();
		rb = GetComponent<Rigidbody>();

		fish.onPlayerFishEvent += FollowPlayer;
		fish.onFishChangeEvent += UpdateBool;
    }

    void FixedUpdate()
	{
		//rb.AddTorque(CalculateMove(neighbours.neighbours) * force);
		if(!usePlayerForce)
        {
			rb.AddTorque(Vector3.Cross(transform.forward, CalculateMove(neighbours.neighboursList)) * force);
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

		Neighbours playerFishNeighbours = playerFish.GetComponent<Neighbours>();
		foreach(GameObject playerNeighbour in playerFishNeighbours.neighboursList)
        {		
			Debug.Log("Test Neighbours");

			playerNeighbour.GetComponent<BoidModel>().neighbourDebugColour = true;
			if(!usePlayerForce)
            {
				usePlayerForce = true;
            }
			rb.AddTorque(playerFish.transform.forward * force);
		}
    }

	void UpdateBool()
    {
		usePlayerForce = false;
    }

}
