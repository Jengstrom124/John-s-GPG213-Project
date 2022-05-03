using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : ManagerBase<LevelInfo>
{
	public Bounds bounds = new Bounds(Vector3.zero, new Vector3(50f,25f,50f));

	public string author;

	public event Action LevelGenerationFinishedEvent;
	protected virtual void OnLevelGenerationFinishedEvent()
	{
		LevelGenerationFinishedEvent?.Invoke();
	}

	private void OnDrawGizmosSelected()
    {
	    Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	    Gizmos.DrawCube(bounds.center, bounds.extents);
    }

}
