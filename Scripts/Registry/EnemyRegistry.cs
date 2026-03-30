using System;
using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Components.Character;

namespace IronStrata.Scripts.Registry;

/// <summary>
/// Data structure defining the base stats and visual properties of an enemy type.
/// </summary>
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
)
{
    public readonly EnemyType Type = type;
    public readonly float Damage = damage;
    public readonly float AttackRange = attackRange;
    public readonly float AttackSpeed = attackSpeed;
    public readonly float Speed = speed;
    public readonly float Health = health;
    public readonly float DispersionRadius = dispersionRadius;
    public readonly Mesh ModelMesh = GD.Load<Mesh>(modelPath);
    public readonly bool IsFlying = isFlying;
    public readonly Vector3 Scale = scale;
    public readonly Color Tint = tint;
    public readonly string Label = label;
}

/// <summary>
/// Rule for how many enemies of a certain type should spawn in a horde.
/// </summary>
public readonly struct HordeSpawnRule(EnemyType type, int count, float chance)
{
    public readonly EnemyType Type = type;
    public readonly int Count = count;
    public readonly float Chance = chance;
}

/// <summary>
/// Central registry for all enemy types and spawning rules.
/// </summary>
public static class EnemyRegistry
{
    /// <summary>
    /// Configuration data for each enemy type.
    /// </summary>
    public static readonly Dictionary<EnemyType, EnemyDefinition> EnemyDefs = new()
    {
        [EnemyType.Crawler] = new EnemyDefinition(
            type: EnemyType.Crawler,
            damage: 10f,
            attackRange: 1f,
            attackSpeed: 1f,
            speed: 7f,
            health: 30f,
            scale: new Vector3(0.02f,  0.02f, 0.02f),
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
            scale: new Vector3(0.02f,  0.02f, 0.02f),
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
            scale: new Vector3(0.02f,  0.02f, 0.02f),
            tint: new Color(0.2f, 0.05f, 0.05f),
            label: "Wasp",
            modelPath: "res://Resources/Assets/Models/Wasp.obj",
            isFlying: true
        )
    };

    /// <summary>
    /// List of rules defining what can spawn during a horde event.
    /// </summary>
    public static readonly List<HordeSpawnRule> SpawnRules =
    [
        new(type: EnemyType.Crawler, count: 20, chance: 1f),
        new(type: EnemyType.Safeguard, count: 10, chance: 0.2f),
        new(type: EnemyType.Wasp, count: 20, chance: 0.5f)
    ];
}