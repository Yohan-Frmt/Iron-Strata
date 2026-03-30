using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Render;

/// <summary>
/// Component for rendering many identical entities using Godot's MultiMeshInstance3D.
/// Ideal for high-performance rendering of swarms or repeated environmental elements.
/// </summary>
public class MultiMeshRenderComponent : IComponent
{
    /// <summary>
    /// The Godot node used for batch rendering.
    /// </summary>
    public MultiMeshInstance3D MultiMeshInstance;

    /// <summary>
    /// The total number of instances currently allocated in the MultiMesh.
    /// </summary>
    public int InstanceCount;

    /// <summary>
    /// The base mesh used for all instances in this multimesh.
    /// </summary>
    public Mesh EnemyMesh;
}