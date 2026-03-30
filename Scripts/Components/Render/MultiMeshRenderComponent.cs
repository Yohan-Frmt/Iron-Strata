using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Render;

public class MultiMeshRenderComponent : IComponent
{
    public MultiMeshInstance3D MultiMeshInstance;
    public int InstanceCount;
    public Mesh EnemyMesh;
}