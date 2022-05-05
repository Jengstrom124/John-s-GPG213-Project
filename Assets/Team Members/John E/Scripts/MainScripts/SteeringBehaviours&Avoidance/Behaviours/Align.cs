using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBase
{
    Rigidbody rb;
	Neighbours neighbours;

	[Header("Align Forwards Direction With Group Forces")]
	public float force = 5f;

	[Header("Do you care about aligning with the player?")]
	public bool alignWithPlayer = true;
	
	[Header("Reference Only/Ignore: ")]
	public Transform currentPlayerFish;

    private void Awake()
    {
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
		if(John.JohnRTSTestController.Instance != null)
        {
			John.JohnRTSTestController.Instance.playerFishSelectedEvent += UpdatePlayerFish;

        }
		else
        {
			Debug.Log("Missing John RTS Controller");
        }
    }

    void FixedUpdate()
	{
		// rb.AddTorque(Vector3.Cross(transform.forward, CalculateMove(neighbours.neighboursList)) * force);
		rb.AddTorque(Vector3.Cross(transform.forward, CalculateMove()) * force);
	}

	// public override Vector3 CalculateMove(List<GameObject> neighbours)
	public override Vector3 CalculateMove()
	{
		int neighboursListCount = neighbours.neighboursList.Count;
		
		if (neighboursListCount == 0)
        {
			return transform.forward;
        }

		Vector3 alignmentMove = Vector3.zero;

		if(currentPlayerFish != null)
        {
			if (neighbours.neighboursList.Contains(currentPlayerFish) && alignWithPlayer)
			{
				alignmentMove = currentPlayerFish.forward;
			}
		}
		else
		{
			// Average of all neighbours directions
			for (var index = 0; index < neighboursListCount; index++)
			{
				// GameObject neighbour = neighbours.neighboursList[index];
				alignmentMove += neighbours.neighboursList[index].forward;
			}

			alignmentMove /= neighbours.neighboursList.Count;
		}

		return alignmentMove;
	}

	void UpdatePlayerFish(GameObject playerFish)
    {
		currentPlayerFish = playerFish.transform;
    }
}
