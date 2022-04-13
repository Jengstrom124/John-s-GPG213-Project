using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StatesEnum : SerializedMonoBehaviour
{
// Acts like a mini-class. You can use this as a TYPE of variable
	public enum States
	{
		Attack,
		RunAway
	}

	public States CurrentState;
	public int health;
	public float distance;

	public IStateBase attack;
	public IStateBase runAway;
	
	public void Update()
	{
		// Also logic for STOPPING ATTACKING
		if (CurrentState == States.Attack && health < 30)
		{
			CurrentState = States.RunAway;
			attack.Enter();
		}

		if (CurrentState == States.RunAway && distance < 10f && health >= 30)
		{
			CurrentState = States.Attack;
			runAway.Enter();
		}
	}
	

}