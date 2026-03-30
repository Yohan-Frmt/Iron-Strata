using System.Linq;
using Godot;
using Godot.Collections;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Render;

public class EnemyMultiMeshSystem(Node3D parent) : ISystem
{
    private readonly Dictionary<EnemyType, MultiMeshInstance3D> _renderers = new();

    public void Update(World world, double delta)
    {
        var enemyEntities = world.Query<EnemyComponent, PositionComponent>().ToList();
        var groups = enemyEntities.GroupBy(e => world.Get<EnemyComponent>(e).Type);

        foreach (var group in groups)
        {
            var type = group.Key;
            var entities = group.ToList();
            if (!_renderers.ContainsKey(type)) _renderers[type] = SetupMultiMesh(type);
            var mm = _renderers[type].Multimesh;
            if (mm.InstanceCount < entities.Count) mm.InstanceCount = entities.Count + 100;
            var def = EnemyRegistry.EnemyDefs[type];
            for (var i = 0; i < entities.Count; i++)
            {
                var pos = world.Get<PositionComponent>(entities[i]).Value;
                var basis = Basis.Identity.Scaled(def.Scale);
                var transform = new Transform3D(basis, pos);
                mm.SetInstanceTransform(i, transform);
            }
            mm.VisibleInstanceCount = entities.Count;
        }

        foreach (var renderer in _renderers)
            if (enemyEntities.All(entity => world.Get<EnemyComponent>(entity).Type != renderer.Key))
                renderer.Value.Multimesh.VisibleInstanceCount = 0;
    }

    private MultiMeshInstance3D SetupMultiMesh(EnemyType type)
    {
        var def = EnemyRegistry.EnemyDefs[type];
        var mm = new MultiMeshInstance3D();
        mm.Multimesh = new MultiMesh();
        mm.Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        mm.Multimesh.Mesh = def.ModelMesh;
        var mat = new StandardMaterial3D { AlbedoColor = def.Tint };
        mm.MaterialOverride = mat;

        parent.AddChild(mm);
        return mm;
    }
}