using Godot;
using IronStrata.Scripts.Components.Train;

namespace IronStrata.Scripts.Core.Data;

/// <summary>
/// Data resource defining base statistics and properties for a wagon type.
/// </summary>
[GlobalClass]
public partial class WagonData : Resource
{
    /// <summary>
    /// Specifies the type of wagon.
    /// </summary>
    [Export] public WagonType Type { get; set; } = WagonType.Storage;

    /// <summary>
    /// Represents the total health points of the wagon.
    /// </summary>
    [Export] public float Health { get; set; } = 150f;

    /// <summary>
    /// Represents the color tint applied to the wagon's appearance.
    /// </summary>
    [Export] public Color Tint { get; set; } = Colors.Gray;

    /// <summary>
    /// Represents a unique identifier or descriptive name assigned to a wagon.
    /// </summary>
    [Export] public string Label { get; set; } = "Wagon";

    [ExportGroup("Combat Properties")]
    /// <summary>
    /// Indicates whether the wagon has a turret.
    /// </summary>
    [Export] public bool HasTurret { get; set; } = false;

    /// <summary>
    /// Defines the attack range of the wagon's turret.
    /// </summary>
    [Export] public float TurretRange { get; set; } = 35f;

    /// <summary>
    /// Represents the amount of damage inflicted by the wagon's turret.
    /// </summary>
    [Export] public float TurretDamage { get; set; } = 15f;

    /// <summary>
    /// Determines the rate at which the wagon's turret can fire.
    /// </summary>
    [Export] public float TurretFireRate { get; set; } = 6f;
}
