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
    void GetEaten(EatAbility eatenBy);
}
