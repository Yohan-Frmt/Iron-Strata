using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

public class TrainMovementComponent : IComponent
{
    public float Speed = 300f;
    public float MaxSpeed = 350f;
    public float Acceleration = 70f;
    public float Deceleration = 220f;
    public float DistanceTraveled = 0f;
    public bool IsBraking = false;
}