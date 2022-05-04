using UnityEngine;
using UnityEngine.InputSystem;

public interface IControllable
{
	// TODO add phase to axis?
	void Steer(float input);
	void Accelerate(float input);
	void Reverse(float input);

	void Action(InputActionPhase aInputActionPhase);
	void Action2(InputActionPhase aInputActionPhase);
	void Action3(InputActionPhase aInputActionPhase);
}

public interface IRTS
{
	void Selected();
	void Deselected();
	void SetDestination(Vector3 position);
}

public enum EdibleType
{
	Food,
	Poison,
	Booster
}

public struct EdibleInfo
{
	public EdibleType edibleType;
	public int amount;
}

// Anything that can be eaten
public interface IEdible
{
	void GetEaten(IPredator eatenBy); // You only need to care about IPredator if you respond to specific types
	EdibleInfo GetInfo();					// Thing that ate it needs to know what TYPE of edible it is (might not be food as such) and the amount it's worth
	void GotShatOut(IPredator shatOutBy);	// Mainly for stupid sound fx etc, BUT might be needed for reenabling physics or whatever
}


// Anything that can eat
// WHY: 
public interface IPredator
{
	Vector3 GetBumPosition();
}

public interface IReactsToWater
{
	public bool IsWet { get; set; }
}

public interface IReactsToInk
{
	public void ChangeInkedState(bool isInked);
}

public interface IStateBase
{
	void Enter();
	void Execute();
	void Exit();
}