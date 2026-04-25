using Godot;
using IronStrata.Scripts.Components.Character;

namespace IronStrata.Scripts.Core.Data;

/// <summary>
/// Data resource defining base statistics and visual properties for an enemy type.
/// </summary>
[GlobalClass]
public partial class EnemyData : Resource
{
    /// <summary>
    /// Specifies the type of enemy.
    /// </summary>
    [Export] public EnemyType Type { get; set; } = EnemyType.Crawler;

    /// <summary>
    /// Represents the amount of damage inflicted by an enemy during an attack.
    /// </summary>
    [Export] public float Damage { get; set; } = 10f;

    /// <summary>
    /// Defines the distance within which an enemy can effectively engage in an attack.
    /// </summary>
    [Export] public float AttackRange { get; set; } = 1f;

    /// <summary>
    /// Determines the rate at which an enemy can perform consecutive attacks (attacks per second).
    /// </summary>
    [Export] public float AttackSpeed { get; set; } = 1f;

    /// <summary>
    /// Represents the movement speed of an enemy.
    /// </summary>
    [Export] public float Speed { get; set; } = 7f;

    /// <summary>
    /// Represents the total health points of the enemy.
    /// </summary>
    [Export] public float Health { get; set; } = 30f;

    /// <summary>
    /// Defines the scale of the enemy in 3D space.
    /// </summary>
    [Export] public Vector3 Scale { get; set; } = new Vector3(0.02f, 0.02f, 0.02f);

    /// <summary>
    /// Represents the color tint applied to an enemy's appearance.
    /// </summary>
    [Export] public Color Tint { get; set; } = new Color(0.1f, 0.1f, 0.1f);

    /// <summary>
    /// Represents a unique identifier or descriptive name assigned to an enemy.
    /// </summary>
    [Export] public string Label { get; set; } = "Enemy";

    /// <summary>
    /// Represents the 3D model mesh associated with an enemy.
    /// </summary>
    [Export] public Mesh ModelMesh { get; set; }

    /// <summary>
    /// Specifies the radius within which enemies are randomly dispersed from their spawn point.
    /// </summary>
    [Export] public float DispersionRadius { get; set; } = 40f;

    /// <summary>
    /// Indicates whether the enemy is capable of flight.
    /// </summary>
    [Export] public bool IsFlying { get; set; } = false;
}
