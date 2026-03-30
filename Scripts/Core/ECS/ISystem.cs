namespace IronStrata.Scripts.Core.ECS;


public interface ISystem
{
    void Update(World world, double delta);
}

public interface IFixedSystem
{
    void FixedUpdate(World world, double delta);
}