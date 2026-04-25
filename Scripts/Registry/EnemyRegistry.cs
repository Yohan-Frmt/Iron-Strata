using System.Collections.Generic;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Core.Data;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Registry;

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
    /// A static dictionary that maps <see cref="EnemyType"/> to their corresponding <see cref="EnemyData"/> objects.
    /// </summary>
    public static Dictionary<EnemyType, EnemyData> EnemyDefs => DataRegistry.EnemyDataMap;

    /// <summary>
    /// Retrieves the definition of an enemy based on its type.
    /// </summary>
    /// <param name="type">The type of the enemy for which the definition is requested.</param>
    /// <returns>
    /// A <see cref="Result{T, E}"/> containing the enemy data if the type exists,
    /// or an error message if the type is not found.
    /// </returns>
    public static Result<EnemyData, string> GetEnemyDefinition(EnemyType type) =>
        EnemyDefs.TryGetValue(type, out EnemyData data)
            ? Result<EnemyData, string>.Ok(data)
            : Result<EnemyData, string>.Err($"Enemy type {type} not found in registry.");

    /// <summary>
    /// Defines the set of rules used to determine the types and quantities of enemies
    /// that can spawn during horde events.
    /// </summary>
    public static readonly List<HordeSpawnRule> SpawnRules = [
        new(type: EnemyType.Crawler, count: 5, chance: 1f),
        new(type: EnemyType.Safeguard, count: 1, chance: 0.2f),
        new(type: EnemyType.Wasp, count: 2, chance: 0.5f)
    ];
}
