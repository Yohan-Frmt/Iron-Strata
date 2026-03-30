using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Render;

/// <summary>
/// Component that enables individual 3D rendering for an entity.
/// </summary>
public class RenderableComponent : IComponent
{
    /// <summary>
    /// The actual Godot Node3D instance in the scene tree.
    /// </summary>
    public Node3D Node;

    /// <summary>
    /// The mesh model associated with this entity.
    /// </summary>
    public Mesh Model;

    /// <summary>
    /// The base color or tint applied to the entity's material.
    /// </summary>
    public Color Tint = new(0.18f, 0.18f, 0.22f);

    /// <summary>
    /// Optional label or name for the entity (e.g., for debugging or UI).
    /// </summary>
    public string Label = "";
}