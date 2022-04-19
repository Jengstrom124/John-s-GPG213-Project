using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeEagle : MonoBehaviour
{
	public bool circuitBreaker;
	public List<Vector3> controlPoints = new (2);

	public Vector3 BezierFunction(List<Vector3> points, float t) 
	{
		List<Vector3> a = points;
		List<Vector3> b = new List<Vector3>(points.Count-1);
		
		for (var i = 0; i < points.Count-1; i++)
		{
			for (var j = 0; j < b.Capacity; j++)
			{
				b.Add(Vector3.Lerp(a[j], a[j + 1], t));
			}

			a = b;
			if (a.Count - 1 != 0)
			{
				b = new List<Vector3>(a.Count - 1);
			}
		}

		//Debug.Log(a[0]);
		return a[0];
	}

	private IEnumerator RunBezier()
	{
		//transform.position = BezierFunction();
		
		
		yield return new WaitForSeconds(0.1f);
		if(!circuitBreaker)
			RunBezier();
	}
	
	private void TurnBody(Vector3 p1, Vector3 p2)
	{
		Vector3 angles = transform.eulerAngles;
		//angles = new Vector3(angles.x, , angles.z);
	}

	private void RandomizeBezierPoints()
	{
		
	}
	
	// Start is called before the first frame update
    void Start()
    {
	    StartCoroutine(RunBezier());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
