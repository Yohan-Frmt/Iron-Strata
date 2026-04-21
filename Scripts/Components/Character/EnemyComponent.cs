using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Components.Character;

/// <summary>
/// Defines the classification of enemies in the game.
/// This enumeration is used to distinguish different types of enemies,
/// influencing their behavior, visual representation, and interactions within the game systems.
/// </summary>
public enum EnemyType { Crawler, Safeguard, Wasp }

/// <summary>
/// Represents the enemy-related behaviors and properties of an entity.
/// This struct is primarily used to define characteristics and combat functionality for enemies
/// within the game world.
/// </summary>
public struct EnemyComponent() {
    /// <summary>
    /// Represents the classification or archetype of an enemy entity within the game.
    /// This value determines specific behavioral patterns, appearance, and other
    /// distinct attributes associated with each enemy type.
    /// It is used across various systems to handle enemy-specific logic, such as
    /// grouping in rendering or deciding attack strategies.
    /// </summary>
    public EnemyType Type = EnemyType.Crawler;

    /// <summary>
    /// Represents the amount of damage an enemy entity can inflict on a target.
    /// This value is used during combat interactions to reduce the health
    /// of the target entity when an attack is performed successfully.
    /// Typically defined during the enemy's initialization and may vary based
    /// on enemy type, difficulty, or other game balancing factors.
    /// </summary>
    public float Damage = 0;

    /// <summary>
    /// Represents the optional target entity that the enemy is currently focused on or interacting with.
    /// This value can either hold a valid target (an Entity) or be empty, depending on the enemy's current state
    /// and whether a suitable target has been identified or remains valid.
    /// Affects behavior such as movement, attack prioritization, and target-related logic.
    /// </summary>
    public Option<Entity> CurrentTarget = Option<Entity>.None;

    /// <summary>
    /// Represents a countdown timer used to regulate the interval between consecutive attacks for an enemy.
    /// This value is decremented over time and resets upon triggering an attack, based on the attack speed.
    /// Ensures that an enemy cannot attack again until the timer reaches zero, enforcing proper cooldown management.
    /// </summary>
    public float AttackTimer = 0f;

    /// <summary>
    /// Defines the rate at which an enemy can perform attacks, measured in attacks per second.
    /// This value is used to determine the cooldown period between consecutive attacks,
    /// influencing the enemy's overall damage output over time.
    /// A higher value results in faster attack intervals, while a lower value introduces
    /// longer delays between attacks.
    /// </summary>
    public float AttackSpeed = 1f;

    /// <summary>
    /// Determines the radius within which the enemy can engage a target in combat.
    /// Used in attack logic to calculate whether a target is within range to be damaged,
    /// and in debugging to visually represent the enemy's attack range in the game world.
    /// The value is expressed in world units.
    /// </summary>
    public float AttackRange = 5f;
}
