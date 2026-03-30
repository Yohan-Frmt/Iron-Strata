using System.Linq;
using Godot;
using Godot.Collections;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Render;

/// <summary>
/// Optimized rendering system for enemies using MultiMeshInstance3D.
/// It batches entities of the same type to reduce draw calls.
/// </summary>
public class EnemyMultiMeshSystem(Node3D parent) : ISystem
{
    /// <summary>
    /// Cache of MultiMeshInstance3D nodes indexed by enemy type.
    /// </summary>
    private readonly Dictionary<EnemyType, MultiMeshInstance3D> _renderers = new();

    /// <summary>
    /// Updates the transforms of all enemy instances in their respective MultiMeshes.
    /// </summary>
    public void Update(World world, double delta)
    {
        var enemyEntities = world.Query<EnemyComponent, PositionComponent>().ToList();
        
        // Group enemies by type to update the corresponding MultiMesh.
        var groups = enemyEntities.GroupBy(e => world.Get<EnemyComponent>(e).Type);

        foreach (var group in groups)
        {
            var type = group.Key;
            var entities = group.ToList();
            
            // Lazy initialization of the MultiMesh for this type.
            if (!_renderers.ContainsKey(type)) 
                _renderers[type] = SetupMultiMesh(type);
            
            var mm = _renderers[type].Multimesh;
            
            // Resize the buffer if needed.
            if (mm.InstanceCount < entities.Count) 
                mm.InstanceCount = entities.Count + 100;
            
            var def = EnemyRegistry.EnemyDefs[type];
            for (var i = 0; i < entities.Count; i++)
            {
                var pos = world.Get<PositionComponent>(entities[i]).Value;
                
                // Construct the transform matrix with scale and position.
                var basis = Basis.Identity.Scaled(def.Scale);
                var transform = new Transform3D(basis, pos);
                
                mm.SetInstanceTransform(i, transform);
            }
            
            // Only draw as many instances as we have entities.
            mm.VisibleInstanceCount = entities.Count;
        }

        // Hide renderers for types that have no active entities.
        foreach (var renderer in _renderers)
        {
            if (enemyEntities.All(entity => world.Get<EnemyComponent>(entity).Type != renderer.Key))
            {
                renderer.Value.Multimesh.VisibleInstanceCount = 0;
            }
        }
    }

    /// <summary>
    /// Configures a new MultiMeshInstance3D for a specific enemy type.
    /// </summary>
    private MultiMeshInstance3D SetupMultiMesh(EnemyType type)
    {
        var def = EnemyRegistry.EnemyDefs[type];
        var mmInstance = new MultiMeshInstance3D();
        var mm = new MultiMesh();
        
        mmInstance.Multimesh = mm;
        mm.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        mm.Mesh = def.ModelMesh;
        
        var mat = new StandardMaterial3D { AlbedoColor = def.Tint };
        mmInstance.MaterialOverride = mat;

        parent.AddChild(mmInstance);
        return mmInstance;
    }
}