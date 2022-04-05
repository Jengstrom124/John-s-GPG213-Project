using UnityEngine;
using Anthill.AI;

public class PickupCargoState : AntAIState
{
	private UnitControl _control;

	public override void Create(GameObject aGameObject)
	{
		_control = aGameObject.GetComponent<UnitControl>();
	}

	public override void Enter()
	{
		// Search cargo on the map and disable it.
		var go = GameObject.Find("Cargo");
		if (go != null)
		{
			go.SetActive(false);
		}

		_control.HasCargo = true;
		Finish();
	}
}

