using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

public class LukeEagle : MonoBehaviour
{
	public bool circuitBreaker;
	public List<Vector3> controlPoints = new(4);
	public int iterationsPerCourse = 200;
	public float durationPerCourse = 7f;
	public float rangePerCourse = 25f;
	private float progress;
	public Vector3 terrainPosition;
	public Vector3 terrainDimensions;

	public Vector3 BezierFunction(List<Vector3> points, float t)
	{
		List<Vector3> a = points;
		List<Vector3> b = new List<Vector3>(points.Count - 1);

		for (var i = 0; i < points.Count - 1; i++)
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
		return a[0];
	}

	private void BezierLooper()
	{
		if (progress > 0)
		{
			StartCoroutine(RunBezier());
		}
		else
		{
			StartCoroutine(BetweenCourses());
		}
	}

	private IEnumerator RunBezier()
	{
		if (circuitBreaker)
		{
			yield break;
		}

		Vector3 tempPosition1 = transform.position;
		Vector3 tempPosition2 = BezierFunction(controlPoints, progress);
		transform.DOMove(tempPosition2, durationPerCourse / iterationsPerCourse);
		progress += 1f / iterationsPerCourse;
		TurnBody(tempPosition1, tempPosition2);
		yield return new WaitForSeconds(durationPerCourse / iterationsPerCourse);
		if (progress >= 1f)
		{
			progress = 0f;
			RandomizeBezierPoints();
		}

		BezierLooper();
	}

	private IEnumerator BetweenCourses()
	{
		float angle = Vector3.SignedAngle(transform.position, BezierFunction(controlPoints, 1f / iterationsPerCourse) - transform.position, Vector3.up);
		transform.DORotate(new Vector3(0, angle, 0), durationPerCourse / 5f);
		yield return new WaitForSeconds(durationPerCourse / 5f);
		StartCoroutine(RunBezier());
	}
	
	private IEnumerator RandomStartTimer()
	{
		yield return new WaitForSeconds(Random.Range(0f, 2f));
		StartCoroutine(RunBezier());
	}

	//smoothing by dividing up into segments of similar length


	private void TurnBody(Vector3 p1, Vector3 p2)
	{
		Vector3 heading = p2 - p1;
		transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
	}

	private void InitializeBezierPoints(List<Vector3> cPs)
	{
		Vector3 position = transform.position;
		cPs.Add(position);
		for (int i = 1; i < cPs.Capacity; i++)
		{
			float controlX = terrainPosition.x + Random.Range(0f, terrainDimensions.x);
			float controlY = terrainPosition.y + 15f;
			float controlZ = terrainPosition.z + Random.Range(0f, terrainDimensions.z);

			controlX = position.x + Mathf.Clamp(controlX - position.x, -rangePerCourse, rangePerCourse);
			controlZ = position.z + Mathf.Clamp(controlZ - position.z, -rangePerCourse, rangePerCourse);

			cPs.Add(new Vector3(controlX, controlY, controlZ));
		}
	}

	private void RandomizeBezierPoints()
	{
		Vector3 position = transform.position;
		controlPoints[0] = controlPoints[^1];
		controlPoints[1] = controlPoints[0] + transform.TransformDirection(new Vector3(Random.Range(-5f, 5f), 0f, 5f));
		for (int i = 2; i < controlPoints.Count; i++)
		{
			float controlX = terrainPosition.x + Random.Range(0f, terrainDimensions.x);
			float controlY = terrainPosition.y + 15f;
			float controlZ = terrainPosition.z + Random.Range(0f, terrainDimensions.z);

			controlX = position.x + Mathf.Clamp(controlX - position.x, -rangePerCourse, rangePerCourse);
			controlZ = position.z + Mathf.Clamp(controlZ - position.z, -rangePerCourse, rangePerCourse);

			controlPoints[i] = new Vector3(controlX, controlY, controlZ);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		TerrainGenerator a = GetComponentInParent<InAirSpawner>().terrainGenerator;
		terrainDimensions = new Vector3(a.width, a.depth, a.height);
		InitializeBezierPoints(controlPoints);
		RandomizeBezierPoints();
		StartCoroutine(RandomStartTimer());
	}
}