using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class UnitSense : MonoBehaviour, ISense
{
	private UnitControl _control;
	private Transform _t;
	private Transform _base;
	private Transform _cargo;

	private void Awake()
	{
		_control = GetComponent<UnitControl>();
		_t = GetComponent<Transform>();
		
		// Save reference for the bot base.
		var go = GameObject.Find("Base");
		Debug.Assert(go != null, "Base object not exists on the scene!");
		_base = go.GetComponent<Transform>();

		// Save reference for the cargo.
		go = GameObject.Find("Cargo");
		Debug.Assert(go != null, "Cargo object not exists on the scene!");
		_cargo = go.GetComponent<Transform>();
	}

	/// <summary>
	/// This method will be called automaticaly each time when AntAIAgent decide to update the plan.
	/// You should attach this script to the same object where is AntAIAgent.
	/// </summary>
	public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
	{
		// 1. Classic setup of the World Conditions for planner.
		// -------------------------------------------------------
		// aWorldState.BeginUpdate(aAgent.planner);
		// {
		// 	aWorldState.Set("Is Cargo Delivered", false);
		// 	aWorldState.Set("See Cargo", IsSeeCargo());
		// 	aWorldState.Set("Has Cargo", _control.HasCargo);
		// 	aWorldState.Set("See Base", IsSeeBase());
		// 	aWorldState.Set("Near Base", IsNearBase());
		// }
		// aWorldState.EndUpdate();

		// 2. Optimized setup of the World Conditions for the planner.
		// -----------------------------------------------------------
		aWorldState.Set(DeliveryBot.IsCargoDelivered, false);
		aWorldState.Set(DeliveryBot.SeeCargo, IsSeeCargo());
		aWorldState.Set(DeliveryBot.HasCargo, _control.HasCargo);
		aWorldState.Set(DeliveryBot.SeeBase, IsSeeBase());
		aWorldState.Set(DeliveryBot.NearBase, IsNearBase());

		// HINT: When you have finished the AI Scenario, just export all conditions
		// as enum and use it to set conditions from the code.
	}

	private bool IsSeeCargo()
	{
		return (AntMath.Distance(_t.position, _cargo.position) < 1.0f);
	}

	private bool IsSeeBase()
	{
		return (AntMath.Distance(_t.position, _base.position) < 1.5f);
	}

	private bool IsNearBase()
	{
		return (AntMath.Distance(_t.position, _base.position) < 0.1f);
	}
}

