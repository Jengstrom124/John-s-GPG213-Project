using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
	public LayerMask layerMask;

	public Terrain terrain;
	
	// Update is called once per frame
	void Update()
	{
		if (Physics.Raycast(transform.position, transform.forward, 100, layerMask.value,
			    QueryTriggerInteraction.Ignore))
		{
			Debug.Log("HIT");
		}
	}

	void LateUpdate()
	{
		Vector3 pos = transform.position;
		pos.y = terrain.SampleHeight(transform.position);
		transform.position = pos;
	}
}