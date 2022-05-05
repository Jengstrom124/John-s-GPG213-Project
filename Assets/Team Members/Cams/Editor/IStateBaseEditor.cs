using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(Cam.StateBase), true)]
// [CustomEditor(typeof(IStateBase), true)] // This SHOULD work, Unity just doesn't support interfaces for some reason. It's dumb.
public class IStateBaseEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("CLICK ME"))
		{
			// HACK: Unity only supplies a normal class "Object" that Monobehaviour inherits from. To access the code implementing an INTERFACE though, you need to generically point the MonoBehaviour script THEN GetComponent using the interface to get it's functions
			((MonoBehaviour) target).GetComponent<IStateBase>().Enter();
		}

	}
}
