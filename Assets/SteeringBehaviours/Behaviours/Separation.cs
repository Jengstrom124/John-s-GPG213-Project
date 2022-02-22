using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBase
{
	Rigidbody rb;
	Neighbours neighbours;

	public float proximityThreshold = 10f;

	private void Start()
	{
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// Some are Torque, some are Force
		rb.AddForce(CalculateMove(neighbours.neighbours));
	}

	public override Vector3 CalculateMove(List<GameObject> neighbours)
	{
		if (neighbours.Count == 0)
		{
			return Vector3.zero;
		}

		Vector3 separationMove = Vector3.zero;

		// Average of all neighbours positions
		foreach (GameObject neighbour in neighbours)
		{
			if (Vector3.Distance(transform.position, neighbour.transform.position) < proximityThreshold)
			{
				separationMove -= transform.InverseTransformPoint(neighbour.transform.position);
			}
		}

		separationMove /= neighbours.Count;

		return separationMove;
	}
}
