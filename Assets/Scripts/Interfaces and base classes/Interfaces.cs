using UnityEngine;

public interface IControllable
{
	void Steer(float      input);
	void Accelerate(float input);
	void Reverse(float    input);
	void Action();
	void Action2();
	void Action3();
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
	public float      amount;
}

// Anything that can be eaten
public interface IEdible
{
	void       GetEaten(IPredator eatenBy);
	EdibleInfo GetInfo();
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