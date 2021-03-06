using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBase
{
	Rigidbody rb;
	Neighbours neighbours;

	[Header("Force to Keep Distance From Neighbours")]
	[Tooltip("At what distance do we start applying forces")]
	public float minDistance = 5f;
	public float force = 2f;

	[Header("Ref Only:")]
	public float myDistance;

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

		Vector3 separationMove = Vector3.zero;
		Vector3 myPos = transform.position;
		Vector3 neighbourPos;

		for (var index = 0; index < neighboursListCount; index++)
		{
			// GameObject neighbour = neighbours.neighboursList[index];
			neighbourPos = neighbours.neighboursList[index].position;

			//Check my Distance from each neighbour
			myDistance = Vector3.Distance(myPos, neighbourPos);

			//if we are too close
			if (myDistance < minDistance)
			{
				separationMove += transform.InverseTransformPoint(neighbourPos);
			}
		}

		separationMove /= neighbours.neighboursList.Count;

		//note seperation move is negative to move away from neighbours
		return -separationMove;
	}
}
