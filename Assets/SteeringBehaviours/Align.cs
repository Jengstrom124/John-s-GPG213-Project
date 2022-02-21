using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : MonoBehaviour
{
    Rigidbody rb;
	Neighbours neighbours;

    private void Start()
    {
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
	{
		// Some are Torque, some are Force
		rb.AddTorque(CalculateMove(neighbours.neighbours));
	}

	public Vector3 CalculateMove(List<GameObject> neighbours)
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
