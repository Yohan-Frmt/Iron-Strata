using Godot;

namespace IronStrata.Scripts.Components.Render;

/// <summary>
/// Represents a visual component that can be rendered within the scene.
/// </summary>
/// <remarks>
/// The <c>RenderableComponent</c> struct is primarily used by systems to visually represent
/// entities in the game world. It contains references to a 3D node, a mesh for the model,
/// a customizable tint color, and an optional label for display purposes.
/// This component is commonly used in conjunction with systems like <c>RenderSystem</c>
/// and <c>WagonHealthUISystem</c> to manage and update the visual state of entities.
/// </remarks>
public struct RenderableComponent() {
    /// <summary>
    /// Represents the root 3D node associated with the renderable entity.
    /// This node serves as the container and transformation root for the
    /// visual representation of the entity within the 3D scene. It allows
    /// interaction with and updates to the graphical components of the entity.
    /// </summary>
    public Node3D Node;

    /// <summary>
    /// Represents the 3D mesh of the renderable component.
    /// This mesh is used as the primary visual representation of the object within the scene.
    /// It defines the geometry that will be drawn by the rendering system.
    /// </summary>
    public Mesh Model;

    /// <summary>
    /// Represents the color tint applied to the rendered surface of an object.
    /// This color is used to customize the visual appearance of renderable components,
    /// such as the wagons in a train system. The tint is typically applied as the
    /// albedo color in the material of a 3D object.
    /// </summary>
    public Color Tint = new(0.18f, 0.18f, 0.22f);

    /// <summary>
    /// Represents the descriptive text or identifier displayed above the rendered object,
    /// such as a wagon in a train system. This label is primarily utilized to provide
    /// debugging information, user-friendly names, or contextual identifiers for visualized
    /// entities within the rendering system.
    /// </summary>
    public string Label = "";
}
