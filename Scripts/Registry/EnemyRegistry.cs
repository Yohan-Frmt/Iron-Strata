using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Registry;

/// <summary>
/// Data structure defining base statistics and visual properties for an enemy type.
/// </summary>
/// <remarks>
/// This structure groups all information necessary for creating and defining the behavior of an enemy,
/// including its combat characteristics, movement, and visual appearance.
/// </remarks>
/// <param name="type">The unique identifier for the enemy type defined by the <see cref="EnemyType"/> enum.</param>
/// <param name="damage">The amount of damage dealt by each successful attack.</param>
/// <param name="attackRange">The maximum distance (in Godot units) at which the enemy can attack.</param>
/// <param name="attackSpeed">The attack rate expressed in attacks per second.</param>
/// <param name="speed">The movement speed of the enemy in units per second.</param>
/// <param name="health">The maximum health points the enemy has when spawned.</param>
/// <param name="scale">The scale factor applied to the enemy's 3D model.</param>
/// <param name="tint">The color tint applied to the model's material.</param>
/// <param name="label">The display name of the enemy, used for UI or debugging.</param>
/// <param name="modelPath">The path to the 3D mesh resource to load.</param>
/// <param name="dispersionRadius">The dispersion radius used during group spawning to prevent overlap.</param>
/// <param name="isFlying">Indicates whether the enemy can fly, affecting navigation and interactions.</param>
public readonly struct EnemyDefinition(
    EnemyType type,
    float damage,
    float attackRange,
    float attackSpeed,
    float speed,
    float health,
    Vector3 scale,
    Color tint,
    string label,
    string modelPath,
    float dispersionRadius = 0f,
    bool isFlying = false
) {
    /// <summary>
    /// Specifies the type of enemy.
    /// </summary>
    /// <remarks>
    /// This value is used to categorize and identify different kinds of enemies within the system.
    /// It serves as a primary identifier for enemy definitions used across the registry and system components.
    /// </remarks>
    public readonly EnemyType Type = type;

    /// <summary>
    /// Represents the amount of damage inflicted by an enemy during an attack.
    /// </summary>
    /// <remarks>
    /// This value is primarily used by the combat system to determine the reduction in health
    /// of a target upon a successful attack. It defines the raw damage output of the enemy unit.
    /// </remarks>
    public readonly float Damage = damage;

    /// <summary>
    /// Defines the distance within which an enemy can effectively engage in an attack.
    /// </summary>
    /// <remarks>
    /// This value is crucial in determining the attack initiation threshold for enemies in the system.
    /// An enemy must be positioned within this range relative to its target to initiate an attack sequence.
    /// It is used across multiple components, such as attack behaviors and engagement mechanics, to enforce this logic.
    /// </remarks>
    public readonly float AttackRange = attackRange;

    /// <summary>
    /// Determines the rate at which an enemy can perform consecutive attacks.
    /// </summary>
    /// <remarks>
    /// This value represents the speed of attack actions, where higher values signify faster attack rates.
    /// It is typically expressed as the number of attacks per second and used to control timing in combat systems.
    /// </remarks>
    public readonly float AttackSpeed = attackSpeed;

    /// <summary>
    /// Represents the movement speed of an enemy.
    /// </summary>
    /// <remarks>
    /// Determines how quickly the enemy can traverse the environment.
    /// This value is applied during the runtime movement calculations to dictate
    /// the velocity of the enemy when approaching targets or navigating the scene.
    /// </remarks>
    public readonly float Speed = speed;

    /// <summary>
    /// Represents the total health points of the enemy.
    /// </summary>
    /// <remarks>
    /// This value determines the enemy's durability and life span in the system.
    /// When reduced to zero, the enemy is defeated and removed from the environment.
    /// It is used in conjunction with combat logic to adjust the state of the enemy based on
    /// interactions such as receiving damage or healing.
    /// </remarks>
    public readonly float Health = health;

    /// <summary>
    /// Specifies the radius within which enemies are randomly dispersed from their spawn point.
    /// </summary>
    /// <remarks>
    /// This value introduces variation in positioning by applying a random offset, allowing for less structured
    /// and more natural-looking enemy group formations. It is particularly useful when spawning clusters of enemies
    /// to create dynamic and unpredictable patterns in their arrangement.
    /// </remarks>
    public readonly float DispersionRadius = dispersionRadius;

    /// <summary>
    /// Represents the 3D model mesh associated with an enemy definition.
    /// </summary>
    /// <remarks>
    /// This mesh is loaded synchronously from the specified `modelPath` when an enemy definition is instantiated.
    /// It is primarily used to visually represent enemies within the game world by assigning it to rendering components,
    /// such as a MultiMesh or mesh instance.
    /// </remarks>
    public readonly Mesh ModelMesh = GD.Load<Mesh>(modelPath);

    /// <summary>
    /// Indicates whether the enemy is capable of flight.
    /// </summary>
    /// <remarks>
    /// This property is used to determine the movement behavior of an enemy.
    /// If set to <c>true</c>, the enemy will be treated as a flying unit,
    /// allowing for specialized movement patterns and positioning in the world.
    /// Flying enemies operate differently in terms of navigation and combat,
    /// impacting their interactions with the environment and player entities.
    /// </remarks>
    public readonly bool IsFlying = isFlying;

    /// <summary>
    /// Defines the scale of the enemy in 3D space.
    /// </summary>
    /// <remarks>
    /// This value determines the size of the enemy's model when rendered within the game environment.
    /// It is used to adjust the enemy's visual proportions and appearance during initialization or updates
    /// by systems such as the rendering system.
    /// </remarks>
    public readonly Vector3 Scale = scale;

    /// <summary>
    /// Represents the color tint applied to an enemy's appearance.
    /// </summary>
    /// <remarks>
    /// This value defines the primary color used to visually distinguish enemies.
    /// It is utilized in rendering systems, such as the material override for enemy models,
    /// to provide unique visual characteristics for different enemy types.
    /// </remarks>
    public readonly Color Tint = tint;

    /// <summary>
    /// Represents a unique identifier or descriptive name assigned to an enemy.
    /// </summary>
    /// <remarks>
    /// This value is used to distinguish different enemies in the system by providing a human-readable name or label.
    /// It aids in debugging, configuration, and management of enemy definitions.
    /// </remarks>
    public readonly string Label = label;
}

/// <summary>
/// Defines a spawn rule for enemy generation during a horde event.
/// </summary>
/// <remarks>
/// Rules are used by the spawn system to determine the composition of an enemy wave.
/// </remarks>
/// <param name="type">The type of enemy to spawn.</param>
/// <param name="count">The number of individuals of this type to generate.</param>
/// <param name="chance">The probability (between 0 and 1) that this rule is applied.</param>
public readonly struct HordeSpawnRule(EnemyType type, int count, float chance) {
    /// <summary>
    /// Represents the category or classification of an enemy.
    /// </summary>
    /// <remarks>
    /// This field is used to identify the specific type of enemy within the system.
    /// It is integral to spawning rules, enemy behavior, and interactions in the game world.
    /// </remarks>
    public readonly EnemyType Type = type;

    /// <summary>
    /// Specifies the number of enemies to be generated based on the spawn rule.
    /// </summary>
    /// <remarks>
    /// This value determines how many individuals of the specified enemy type
    /// are spawned during a horde event. The count is directly used by the
    /// spawning system to populate the game world with enemies as per the
    /// defined rules.
    /// </remarks>
    public readonly int Count = count;

    /// <summary>
    /// Represents the probability of applying a specific spawn rule during a horde event.
    /// </summary>
    /// <remarks>
    /// The value ranges between 0.0 and 1.0, where 0.0 indicates the rule is never applied,
    /// and 1.0 signifies the rule is always applied. This is used to randomize enemy composition
    /// for a more dynamic horde generation system.
    /// </remarks>
    public readonly float Chance = chance;
}

/// <summary>
/// Centralized registry managing enemy types and spawn rules.
/// </summary>
/// <remarks>
/// This static class serves as a read-only database for the game,
/// providing quick access to configurations for each existing enemy type.
/// </remarks>
public static class EnemyRegistry {
    /// <summary>
    /// A static dictionary that maps <see cref="EnemyType"/> to their corresponding <see cref="EnemyDefinition"/> objects.
    /// </summary>
    /// <remarks>
    /// This dictionary serves as a registry for defining the characteristics and behavior of different enemy types.
    /// Each entry in the dictionary provides details such as damage, health, speed, and additional properties specific to the enemy type.
    /// It allows for organized and centralized management of enemy definitions within the system.
    /// </remarks>
    public static readonly Dictionary<EnemyType, EnemyDefinition> EnemyDefs = new() {
        [EnemyType.Crawler] = new EnemyDefinition(
            type: EnemyType.Crawler,
            damage: 10f,
            attackRange: 1f,
            attackSpeed: 1f,
            speed: 7f,
            health: 30f,
            scale: new Vector3(0.02f, 0.02f, 0.02f),
            tint: new Color(0.1f, 0.1f, 0.1f),
            label: "Crawler",
            modelPath: "res://Resources/Assets/Models/Crawler.obj",
            dispersionRadius: 40f
        ),

        [EnemyType.Safeguard] = new EnemyDefinition(
            type: EnemyType.Safeguard,
            damage: 50f,
            attackRange: 10f,
            attackSpeed: 0.5f,
            speed: 3f,
            health: 300f,
            label: "Safeguard",
            scale: new Vector3(0.02f, 0.02f, 0.02f),
            tint: new Color(0.2f, 0.05f, 0.05f),
            modelPath: "res://Resources/Assets/Models/Safeguard.obj"
        ),

        [EnemyType.Wasp] = new EnemyDefinition(
            type: EnemyType.Wasp,
            damage: 5f,
            attackRange: 10f,
            attackSpeed: 2f,
            speed: 10f,
            health: 10f,
            scale: new Vector3(0.02f, 0.02f, 0.02f),
            tint: new Color(0.2f, 0.05f, 0.05f),
            label: "Wasp",
            modelPath: "res://Resources/Assets/Models/Wasp.obj",
            isFlying: true
        )
    };

    /// <summary>
    /// Retrieves the definition of an enemy based on its type.
    /// </summary>
    /// <param name="type">The type of the enemy for which the definition is requested.</param>
    /// <returns>
    /// A <see cref="Result{T, E}"/> containing the enemy definition if the type exists,
    /// or an error message if the type is not found.
    /// </returns>
    public static Result<EnemyDefinition, string> GetEnemyDefinition(EnemyType type) =>
        EnemyDefs.TryGetValue(type, out EnemyDefinition def)
            ? Result<EnemyDefinition, string>.Ok(def)
            : Result<EnemyDefinition, string>.Err($"Enemy type {type} not found in registry.");

    /// <summary>
    /// Defines the set of rules used to determine the types and quantities of enemies
    /// that can spawn during horde events.
    /// </summary>
    /// <remarks>
    /// This list contains predefined spawn configurations where each rule specifies
    /// the enemy type, the number of enemies to spawn, and the probability of spawning.
    /// It is utilized by the game system to dynamically populate enemies during gameplay.
    /// </remarks>
    public static readonly List<HordeSpawnRule> SpawnRules = [
        new(type: EnemyType.Crawler, count: 20, chance: 1f), new(type: EnemyType.Safeguard, count: 10, chance: 0.2f),
        new(type: EnemyType.Wasp, count: 20, chance: 0.5f)
    ];
}
