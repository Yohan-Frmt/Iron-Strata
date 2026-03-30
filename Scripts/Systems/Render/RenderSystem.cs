using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Render;

public class RenderSystem : ISystem
{
    private readonly Node3D _trainRoot;

    public RenderSystem(Node3D trainRoot, World world)
    {
        _trainRoot = trainRoot;
        world.OnEntityDestroyed += OnEntityDestroyed;
    }

    public void Update(World world, double delta)
    {
        var distance = world.Query<TrainMovementComponent>().Select(mv => world.Get<TrainMovementComponent>(mv).DistanceTraveled).FirstOrDefault();

        foreach (var entity in world.Query<RenderableComponent, WagonSlotComponent>())
        {
            var r = world.Get<RenderableComponent>(entity);
            var slot = world.Get<WagonSlotComponent>(entity);

            if (r.Node == null)
            {
                r.Node = BuildSafeWagon(entity, r);
                _trainRoot.AddChild(r.Node);
            }

            var bobbingPhase = (distance * 0.5f) + (slot.SlotIndex * 1.5f);
            var bobY = Mathf.Sin(bobbingPhase) * 0.1f; 
            var basePos = TrainLayout.GetLocalPosition(slot.SlotIndex, slot.Layer);
            r.Node.Position = basePos + new Vector3(0, bobY, 0);
        }
    }

    private static Node3D BuildSafeWagon(Entity entity, RenderableComponent r)
    {
        var root = new Node3D { Name = $"Wagon_{entity.Id}" };

        var mesh = new MeshInstance3D { Name = "Body" };
        mesh.Mesh = new BoxMesh { Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth) };
        
        var mat = new StandardMaterial3D { AlbedoColor = r.Tint };
        mesh.SetSurfaceOverrideMaterial(0, mat);
        root.AddChild(mesh);

        var label3D = new Label3D
        {
            Text = r.Label,
            FontSize = 24,
            Position = new Vector3(0, 2.5f, 0),
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled
        };
        root.AddChild(label3D);
        
        var area = new Area3D();
        var collision = new CollisionShape3D();
        collision.Shape = new BoxShape3D { Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth) };
        area.AddChild(collision);
        area.SetMeta("EntityId", entity.Id);
        root.AddChild(area);
        return root;
    }

    private void OnEntityDestroyed(Entity entity)
    {
        var node = _trainRoot.GetNodeOrNull<Node3D>($"Wagon_{entity.Id}");
        node?.QueueFree();
    }
}