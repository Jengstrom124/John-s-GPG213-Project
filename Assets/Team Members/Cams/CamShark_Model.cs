using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TailSection
{
	public Transform bone;
	public SectionPhysics sectionPhysics;
}

public class CamShark_Model : MonoBehaviour, IControllable, IPredator, IEdible
{

	public List<TailSection> tailSections;
	public Rigidbody rb;

	private void Awake()
	{
		foreach (TailSection tailSection in tailSections)
		{
			// tailSection.bone.gameObject.AddComponent<Rigidbody>();
			tailSection.sectionPhysics = tailSection.bone.gameObject.AddComponent<SectionPhysics>();
			tailSection.sectionPhysics.mainBody = rb;
		}
	}


	public void Steer(float input)
    {
	    foreach (TailSection tailSection in tailSections)
	    {
		    tailSection.bone.Rotate(new Vector3(0,0, input*20f), Space.Self);
	    }
    }

    public void Accelerate(float input)
    {
    }

    public void Reverse(float input)
    {
    }

    public void Action()
    {
    }

    public void Action2()
    {
    }

    public void Action3()
    {
    }

    public void GotFood(float amount)
    {
    }

    public void ChangeBoost(float amount)
    {
    }

    public void GetEaten(IPredator eatenBy)
    {
    }

	public EdibleInfo GetInfo()
	{
		return new EdibleInfo();
	}

	public Vector3 GetBumPosition()
	{
		return Vector3.zero;
	}
}
