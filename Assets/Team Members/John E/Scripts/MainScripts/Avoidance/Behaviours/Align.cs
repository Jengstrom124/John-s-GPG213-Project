using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBase
{
    Rigidbody rb;
	Neighbours neighbours;

	[Header("Align Forwards Direction With Group Forces")]
	public float force = 5f;
	
	//Player Fish Stuff
	[Header("Ignore this - was testing")]
	public bool usePlayerForce = false;
	public GameObject currentPlayerFish;

    private void Awake()
    {
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
	{
		//rb.AddTorque(CalculateMove(neighbours.neighbours) * force);
		if(!usePlayerForce || currentPlayerFish != null)
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
}
