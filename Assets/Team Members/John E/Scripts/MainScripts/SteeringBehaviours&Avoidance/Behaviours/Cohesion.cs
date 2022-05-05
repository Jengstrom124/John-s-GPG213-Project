using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : SteeringBase
{
	Rigidbody rb;
	Neighbours neighbours;

	[Header("Group With Neighbour Forces")]
	public float force = 0.5f;

	private void Start()
	{
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{		
		// rb.AddRelativeForce(CalculateMove(neighbours.neighboursList) * force);
		rb.AddRelativeForce(CalculateMove() * force);
	}

	// public override Vector3 CalculateMove(List<GameObject> neighbours)
	public override Vector3 CalculateMove()
	{
		int neighboursListCount = neighbours.neighboursList.Count;
		
		if (neighboursListCount == 0)
        {
			return Vector3.zero;
        }

		Vector3 cohesionMove = Vector3.zero;

		// Average of all neighbours positions
		for (var index = 0; index < neighboursListCount; index++)
		{
			// GameObject neighbour = neighbours.neighboursList[index];
			cohesionMove += transform.InverseTransformPoint(neighbours.neighboursList[index].position);
		}

		cohesionMove /= neighbours.neighboursList.Count;

		return cohesionMove;
	}
}
