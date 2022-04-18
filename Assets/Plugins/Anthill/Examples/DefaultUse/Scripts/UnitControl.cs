using Anthill.Utils;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
	public GameObject backpackRef;
	private bool _hasCargo;
	
	
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
	
	public bool HasCargo
	{
		get { return _hasCargo; }
		set 
		{
			_hasCargo = value;
			backpackRef.SetActive(_hasCargo);
		}
	}

	public bool IsSeeCargo()
	{
		return (AntMath.Distance(_t.position, _cargo.position) < 1.0f);
	}

	public bool IsSeeBase()
	{
		return (AntMath.Distance(_t.position, _base.position) < 1.5f);
	}

	public bool IsNearBase()
	{
		return (AntMath.Distance(_t.position, _base.position) < 0.1f);
	}

}

