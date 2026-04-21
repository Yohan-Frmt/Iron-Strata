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
public class EnemyMultiMeshSystem(Node3D parent) : ISystem {
    /// <summary>
    /// Cache of MultiMeshInstance3D nodes indexed by enemy type.
    /// </summary>
    private readonly Dictionary<EnemyType, MultiMeshInstance3D> _renderers = [];

    /// <summary>
    /// Cache of entities grouped by enemy type to avoid allocations during Update.
    /// </summary>
    private readonly Dictionary<EnemyType, List<Entity>> _enemyGroups = [];

    /// <summary>
    /// Updates the enemy MultiMesh rendering system by organizing entities into groups based on their enemy type
    /// and configuring MultiMesh instances for rendering.
    /// </summary>
    /// <param name="world">The game world containing entities and components required for processing.</param>
    /// <param name="delta">The time elapsed since the last update, used for time-dependent calculations.</param>
    public void Update(World world, double delta) {
        foreach (List<Entity> list in _enemyGroups.Values) { list.Clear(); }

        foreach (Entity entity in world.Query<EnemyComponent, PositionComponent>()) {
            EnemyType type = world.Get<EnemyComponent>(entity).Type;
            if (!_enemyGroups.TryGetValue(type, out List<Entity> enemyList)) {
                enemyList = [];
                _enemyGroups[type] = enemyList;
            }
            enemyList.Add(entity);
        }

        foreach ((EnemyType type, List<Entity> entities) in _enemyGroups) {
            if (entities.Count == 0) {
                if (_renderers.TryGetValue(type, out MultiMeshInstance3D renderer)) { renderer.Multimesh.VisibleInstanceCount = 0; }
                continue;
            }

            if (!_renderers.TryGetValue(type, out MultiMeshInstance3D rendererInstance)) { rendererInstance = SetupMultiMesh(type); _renderers[type] = rendererInstance; }

            MultiMesh multimesh = rendererInstance.Multimesh;

            if (multimesh.InstanceCount < entities.Count) { multimesh.InstanceCount = entities.Count + 100; }

            EnemyDefinition enemyDefinition = EnemyRegistry.EnemyDefs[type];
            for (int entityIndex = 0; entityIndex < entities.Count; entityIndex++) {
                Vector3 position = world.Get<PositionComponent>(entities[entityIndex]).Value;
                Basis basis = Basis.Identity.Scaled(enemyDefinition.Scale);
                Transform3D transform = new(basis, position);
                multimesh.SetInstanceTransform(entityIndex, transform);
            }
            multimesh.VisibleInstanceCount = entities.Count;
        }
    }

    /// <summary>
    /// Configures and initializes a MultiMeshInstance3D for the specified enemy type.
    /// </summary>
    /// <param name="type">The type of enemy for which the MultiMeshInstance3D is set up.</param>
    /// <returns>
    /// A new MultiMeshInstance3D instance configured with the appropriate model, material,
    /// and transformation settings for the given enemy type.
    /// </returns>
    private MultiMeshInstance3D SetupMultiMesh(EnemyType type) {
        EnemyDefinition definition = EnemyRegistry.EnemyDefs[type];
        MultiMeshInstance3D multiMeshInstance = new();
        MultiMesh multiMesh = new();

        multiMeshInstance.Multimesh = multiMesh;
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.Mesh = definition.ModelMesh;

        StandardMaterial3D material = new() { AlbedoColor = definition.Tint };
        multiMeshInstance.MaterialOverride = material;

        parent.AddChild(multiMeshInstance);
        return multiMeshInstance;
    }
}
