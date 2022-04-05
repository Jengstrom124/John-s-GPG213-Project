using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class UnloadCargoState : AntAIState
{
	private UnitControl _control;
	private GameObject _cargoRef;
	private Vector3 _initialPos;

	public override void Create(GameObject aGameObject)
	{
		// Save reference to the cargo to respawn it when
		// unity finish delivery.
		_cargoRef = GameObject.Find("Cargo");
		Debug.Assert(_cargoRef != null, "Cargo not found on the scene!");
		_initialPos = _cargoRef.transform.position;

		_control = aGameObject.GetComponent<UnitControl>();
	}

	public override void Enter()
	{
		// Spawn cargo again on the map in the random pos.
		_cargoRef.transform.position = new Vector3(
			_initialPos.x + AntMath.RandomRangeFloat(-2.0f, 2.0f),
			_initialPos.y + AntMath.RandomRangeFloat(-2.0f, 2.0f),
			0.0f
		);
		_cargoRef.SetActive(true);

		_control.HasCargo = false;
		Finish();
	}
}

