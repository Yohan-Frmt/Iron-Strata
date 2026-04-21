using Godot;

namespace IronStrata.Scripts.Components.Map;

/// <summary>
/// Represents a component that associates an OmniLight3D node with an entity in the ECS system.
/// This component is specifically used for managing rail-associated lights within a game world.
/// </summary>
/// <remarks>
/// The RailLightComponent contains an OmniLight3D light node, which is used to configure the light's
/// properties, such as its position, color, intensity, shadow settings, and volumetric fog energy.
/// </remarks>
public struct RailLightComponent {
    /// <summary>
    /// Represents a reference to an OmniLight3D node within the ECS system.
    /// </summary>
    /// <remarks>
    /// This field is used to associate an OmniLight3D instance with its corresponding entity
    /// in the game world. It enables manipulation and configuration of the light node
    /// as part of an entity-component-system architecture. The OmniLight3D manages
    /// properties such as light color, intensity, position, and shadow settings specific to rail lighting scenarios.
    /// </remarks>
    public OmniLight3D LightNode;
}
