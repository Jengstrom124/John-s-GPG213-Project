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
		rb.AddRelativeForce(CalculateMove(neighbours.neighboursList) * force);
	}

	public override Vector3 CalculateMove(List<GameObject> neighbours)
	{
		if (neighbours.Count == 0)
		{
			return Vector3.zero;
		}

		Vector3 separationMove = Vector3.zero;

		foreach (GameObject neighbour in neighbours)
		{
			//Check my Distance from each neighbour
			myDistance = Vector3.Distance(transform.position, neighbour.transform.position);

			//if we are too close
			if(myDistance < minDistance)
            {
				separationMove += transform.InverseTransformPoint(neighbour.transform.position);
            }

		}

		separationMove /= neighbours.Count;

		//note seperation move is negative to move away from neighbours
		return -separationMove;
	}
}
