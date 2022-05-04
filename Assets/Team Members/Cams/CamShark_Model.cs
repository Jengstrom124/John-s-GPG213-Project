using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class TailSection
{
	public Transform bone;
	public SectionPhysics sectionPhysics;
}

public class CamShark_Model : NetworkBehaviour, IControllable, IPredator, IEdible
{

	public List<TailSection> tailSections;
	public Rigidbody rb;
	private float speed;

	private void Awake()
	{
		foreach (TailSection tailSection in tailSections)
		{
			// tailSection.bone.gameObject.AddComponent<Rigidbody>();
			tailSection.sectionPhysics = tailSection.bone.gameObject.AddComponent<SectionPhysics>();
			tailSection.sectionPhysics.mainBody = rb;
		}
		
		DoAThing();
	}


	private void Update()
	{

	}

	private void FixedUpdate()
	{
		transform.Translate(0,0,speed);
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
	    // Boost

	    if (IsClient)
	    {
		    // change colour red		v
		    // playAnimation			v
		    // play sound				v
		    // funky particle effect	v
	    }

	    if (IsServer)
	    {
		    // accel++					m
		    // shit out fish			m
	    }
	     
    }

    public async void DoAThing()
    {
	    Debug.Log("Before async");
	    await Task.Delay(1000);
	    Debug.Log("After async");
    }

    public void Action2()
    {
    }

    public void Action3()
    {
    }

    public void GetEaten(IPredator eatenBy)
    {
    }

	public EdibleInfo GetInfo()
	{
		return new EdibleInfo();
	}

	public void GotShatOut(IPredator shatOutBy)
	{
		
	}

	public Vector3 GetBumPosition()
	{
		return Vector3.zero;
	}
}
