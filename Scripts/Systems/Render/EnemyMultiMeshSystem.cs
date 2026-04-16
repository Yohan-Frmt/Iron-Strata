using System.Collections.Generic;
using Godot;
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
    /// Cache of entities grouped by enemy type to avoid allocations during Update.
    /// </summary>
    private readonly Dictionary<EnemyType, List<Entity>> _enemyGroups = new();

    /// <summary>
    /// Updates the transforms of all enemy instances in their respective MultiMeshes.
    /// </summary>
    public void Update(World world, double delta)
    {
        // Clear previous groups
        foreach (var list in _enemyGroups.Values) list.Clear();

        // Categorize enemies by type
        foreach (var entity in world.Query<EnemyComponent, PositionComponent>())
        {
            var type = world.Get<EnemyComponent>(entity).Type;
            if (!_enemyGroups.TryGetValue(type, out var list))
            {
                list = new List<Entity>();
                _enemyGroups[type] = list;
            }
            list.Add(entity);
        }

        foreach (var pair in _enemyGroups)
        {
            var type = pair.Key;
            var entities = pair.Value;
            
            if (entities.Count == 0)
            {
                if (_renderers.TryGetValue(type, out var r)) 
                    r.Multimesh.VisibleInstanceCount = 0;
                continue;
            }

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