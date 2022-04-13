public interface IControllable
{
    void Steer(float input);
    void Accelerate(float input);
    void Reverse(float input);
    void Action();
    void Action2();
    void Action3();
}

public interface IEdible
{
    void GetEaten(IPredator eatenBy);
}

public interface IPredator
{
	void GotFood(float amount);
	void ChangeBoost(float amount);
}

public interface IReactsToWater
{
	public bool IsWet { get; set; }
}

public interface IStateBase
{
	void Enter();
	void Execute();
	void Exit();
}