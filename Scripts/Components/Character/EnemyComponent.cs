using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Components.Character;

/// <summary>
/// Defines the various types of enemies.
/// </summary>
public enum EnemyType { Crawler, Safeguard, Wasp }

/// <summary>
/// Component that marks an entity as an enemy and stores its combat-related data.
/// </summary>
public class EnemyComponent : IComponent
{
    /// <summary>
    /// The specific type of enemy.
    /// </summary>
    public EnemyType Type;

    /// <summary>
    /// The amount of damage this enemy deals.
    /// </summary>
    public float Damage;

    /// <summary>
    /// The current target entity this enemy is attacking or pursuing.
    /// </summary>
    public Option<Entity> CurrentTarget = Option<Entity>.None; 

    /// <summary>
    /// Current cooldown timer for attacks.
    /// </summary>
    public float AttackTimer = 0f;

    /// <summary>
    /// The time between attacks in seconds.
    /// </summary>
    public float AttackSpeed = 1f;

    /// <summary>
    /// The maximum range at which this enemy can attack its target.
    /// </summary>
    public float AttackRange = 5f;
}