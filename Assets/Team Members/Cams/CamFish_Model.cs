using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFish_Model : MonoBehaviour, IEdible, IReactsToWater
{
	public float amount;

	private event Action<int> ChangedScoreEvent;

	private void Start()
	{
		IsWet = true;
	}

	private int score;
	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			ChangedScoreEvent?.Invoke(value);
			score = value;
		}
	}


	public void GetEaten(IPredator eatenBy)
	{
		Destroy(gameObject);
    }

	public EdibleInfo GetInfo()
	{
		EdibleInfo edibleInfo = new EdibleInfo();
		edibleInfo.edibleType = EdibleType.Food;
		edibleInfo.amount = 1;
		
		return edibleInfo;
	}

	public bool IsWet { get; set; }
}
