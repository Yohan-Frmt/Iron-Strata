using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Character;

public enum EnemyType { Crawler, Safeguard, Wasp }

public class EnemyComponent : IComponent
{
    public EnemyType Type;
    public float Damage;
    public Entity CurrentTarget; 
    public float AttackTimer = 0f;
    public float AttackSpeed = 1f;
    public float AttackRange = 5f;
}