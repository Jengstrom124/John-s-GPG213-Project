using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
	public GameObject cubePrefab;
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

	void Start()
	{
		for (int x = 0; x < 256; x++)
		{
			for (int z = 0; z < 256; z++)
			{
				Vector3 pos = new Vector3(x,0,z);
				pos.y = terrain.SampleHeight(pos);
				if (pos.y>16f)
				{
					Instantiate(cubePrefab, pos, Quaternion.identity);
					
				}
				// transform.position = pos;
			}
		}
	}
}