using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAway : MonoBehaviour, IStateBase
{
	

	public void Enter()
	{
		// Once, play 'RUNAWAY!' sound
		Debug.Log("Runaway Enter");
	}

	public void Execute()
	{
		
	}

	public void Exit()
	{
		
	}
}
