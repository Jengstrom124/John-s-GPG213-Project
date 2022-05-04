using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnTestSpawner : MonoBehaviour
{
	public bool spawnFacingForward;
	public bool autoSpawnAtStart;

	[Space]
	public WaterOrNot waterOrNot = WaterOrNot.Water;
	/// <summary>
	/// When spawning 'on the water' do we ACTUALLY want them to be SLIGHTLY below the water?
	/// </summary>
	public float waterOffset = -0.5f;
	public PositionCalculation positionCalculation = PositionCalculation.useLevelInfoBounds;

	public int spawnCount;
	public float fudgeScale = 1f;


	[Header("Configurations:")]
	public List<GameObject> objectsToSpawn;

	//Spawn Area
	[Header("Custom Spawn Area (Not using SpawnerPos)")]
	public Vector3Int xyzMin = new Vector3Int(0, 0, 0);

	public Vector3Int xyzMax = new Vector3Int(50, 50, 50);

	[Header("Adjust Only When Using Spawner Pos (Adds to current position)")]
	public int xMax = 50;
	public int zMax = 50;



	public enum WaterOrNot
	{
		Water,
		Seabed,
		Land,
		Air,
		Both,
		FixedHeight
	}

	public enum PositionCalculation
	{
		useSpawnerPositionPlusMinMax,
		randomUsingMinMax,
		useLevelInfoBounds
	}


	// Start is called before the first frame update
	void Start()
	{
		if (autoSpawnAtStart)
			SpawnStuff();
	}

	public void SpawnStuff()
	{
		for (int i = 0; i < spawnCount; i++)
		{
			GameObject newObject = Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Count - 1)]) as GameObject;
			//newObject.GetComponent<NetworkObject>().Spawn();
			newObject.transform.localScale = new Vector3(fudgeScale, fudgeScale, fudgeScale);


			Vector3 finalPosition = CalculateRandomPosition(waterOrNot);


			newObject.transform.position = finalPosition;


			if (spawnFacingForward)
			{
				newObject.transform.rotation = Quaternion.Euler(transform.forward);
			}
			else
			{
				newObject.transform.rotation = Quaternion.Euler(newObject.transform.rotation.x, Random.Range(1, 359), newObject.transform.rotation.z);
			}
		}
	}


	Vector3 finalPosition;
	public Vector3 CalculateRandomPosition(WaterOrNot _waterOrNot)
	{
		bool foundGoodSpot = false;
		int maxLoopCount = 1000;

		int loopCount = 0;

		while (!foundGoodSpot && loopCount < maxLoopCount)
		{
			loopCount++;

			if (positionCalculation == PositionCalculation.useSpawnerPositionPlusMinMax)
			{
				finalPosition =
					new Vector3(Random.Range(transform.position.x - xMax / 2f, transform.position.x + xMax / 2f), 0,
						Random.Range(transform.position.z - zMax / 2f, transform.position.z + zMax / 2f));
			}
			else
			{
				finalPosition = new Vector3(Random.Range(xyzMax.x, xyzMax.x), 0,
					Random.Range(xyzMax.z, xyzMax.z));
			}

			if (positionCalculation == PositionCalculation.useLevelInfoBounds)
			{
				float xRnd = Random.Range(LevelInfo.Instance.bounds.center.x + -LevelInfo.Instance.bounds.extents.x / 2f, LevelInfo.Instance.bounds.center.x + LevelInfo.Instance.bounds.extents.x / 2f);
				float zRnd = Random.Range(LevelInfo.Instance.bounds.center.z + -LevelInfo.Instance.bounds.extents.z / 2f, LevelInfo.Instance.bounds.center.z + LevelInfo.Instance.bounds.extents.z / 2f);

				finalPosition = new Vector3(xRnd, 0, zRnd);
			}

			// Always set y to highest point on LevelInfo
			finalPosition.y = LevelInfo.Instance.bounds.center.y + LevelInfo.Instance.bounds.extents.y / 2f;


			if (_waterOrNot == WaterOrNot.Water)
			{
				RaycastHit HitInfo;
				if (Physics.Raycast(new Ray(finalPosition, Vector3.down), out HitInfo, 999999f, 255, QueryTriggerInteraction.Collide)) ;
				{
					if (HitInfo.transform.GetComponent<Water>())
					{
						finalPosition.y = HitInfo.point.y + waterOffset;
						foundGoodSpot = true;
					}
				}
			}
			if (_waterOrNot == WaterOrNot.Land)
			{
				RaycastHit HitInfo;
				if (Physics.Raycast(new Ray(finalPosition, Vector3.down), out HitInfo, 999999f, 255, QueryTriggerInteraction.Collide)) ;
				{
					if (HitInfo.transform.GetComponent<Terrain>())
					{
						finalPosition.y = HitInfo.point.y;
						foundGoodSpot = true;
					}
				}
			}
			if (_waterOrNot == WaterOrNot.Seabed)
			{
				RaycastHit HitInfo;
				RaycastHit[] raycastHits = Physics.RaycastAll(new Ray(finalPosition, Vector3.down), 999999f, 255, QueryTriggerInteraction.Collide);

				if (raycastHits.Length > 1) ;
				{
					if (raycastHits[0].transform.GetComponent<Water>())
					{
						finalPosition.y = raycastHits[1].point.y;
						foundGoodSpot = true;
					}
				}
			}

		}

		return finalPosition;
	}
}
