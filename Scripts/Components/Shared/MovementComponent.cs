using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

public class MovementComponent : IComponent
{
    public float Speed;
    public float TurnSpeed;
    public bool IsFlying;
}