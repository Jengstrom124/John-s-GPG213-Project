using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Attack : Cam.StateBase, IStateBase
{
	public float damage = 10f;

	public StatesEnum camsModelScript;
	
	public void Enter()
	{
		
		Debug.Log("Attack Enter");
		
		// Logic for attacking
		// ???? MAYBE ??? Logic for deciding to RunAway
		
		// ONCE play "ATTACK!" voice line
		//		We need to be able to ENTER, EXECUTE and EXIT a states with separate code for each
	}

	[Button]
	public void Execute()
	{
		
	}

	public void Exit()
	{
		
	}
}
