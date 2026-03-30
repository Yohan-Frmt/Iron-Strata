using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

public class TurretComponent : IComponent
{
    public float Range = 25f;
    public float Damage = 15f;
    public float FireRate = 5.0f;
    public float Cooldown = 0f;
}