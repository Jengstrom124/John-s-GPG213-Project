using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class UnitSense : MonoBehaviour, ISense
{
	private UnitControl _control;

	private void Awake()
	{
		_control = GetComponent<UnitControl>();
	}

	/// <summary>
	/// This method will be called automaticaly each time when AntAIAgent decide to update the plan.
	/// You should attach this script to the same object where is AntAIAgent.
	/// </summary>
	public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
	{
		aWorldState.Set(DeliveryBot.IsCargoDelivered, false);
		aWorldState.Set(DeliveryBot.SeeCargo, _control.IsSeeCargo());
		aWorldState.Set(DeliveryBot.HasCargo, _control.HasCargo);
		aWorldState.Set(DeliveryBot.SeeBase, _control.IsSeeBase());
		aWorldState.Set(DeliveryBot.NearBase, _control.IsNearBase());

		// HINT: When you have finished the AI Scenario, just export all conditions
		// as enum and use it to set conditions from the code.
	}

}

