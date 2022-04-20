using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSharkIntoWater : MonoBehaviour
{
	public GameObject shark;
	public TerrainGenerator terrainGenerator;
	
	// Start is called before the first frame update
	void Awake()
	{
		Vector3 target = new Vector3(Random.Range(0f,terrainGenerator.width), 9.1f, Random.Range(0f,terrainGenerator.height));
		
		Ray ray1 = new Ray(target, Vector3.up);
		Ray ray2 = new Ray(target, Vector3.forward);
		Ray ray3 = new Ray(target, Vector3.back);
		RaycastHit raycastHit;
		
		while (Physics.Raycast(ray1, out raycastHit, 10f) || Physics.Raycast(ray2, out raycastHit, 2.5f) || Physics.Raycast(ray3, out raycastHit, 4.5f))
		{
			target = new Vector3(Random.Range(0f,terrainGenerator.width), 9.1f, Random.Range(0f,terrainGenerator.height));
		}

		shark.transform.position = target;
	}
}