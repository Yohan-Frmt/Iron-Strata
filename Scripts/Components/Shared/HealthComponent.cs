using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

public partial class HealthComponent : IComponent
{
    public float Max;
    public float Current;
    public bool IsDestroyed => Current <= 0f;
}