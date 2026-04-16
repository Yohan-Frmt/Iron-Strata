using Godot;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Render;

/// <summary>
/// Main rendering system for the train wagons.
/// It handles the creation of physical Godot nodes for entities and applies visual effects like bobbing.
/// </summary>
public class RenderSystem : ISystem
{
    private readonly Node3D _trainRoot;

    /// <summary>
    /// Initializes a new instance of the RenderSystem and hooks into entity destruction events.
    /// </summary>
    public RenderSystem(Node3D trainRoot, World world)
    {
        _trainRoot = trainRoot;
        world.OnEntityDestroyed += OnEntityDestroyed;
    }

    /// <summary>
    /// Updates the position and visual state of all renderable wagons.
    /// </summary>
    public void Update(World world, double delta)
    {
        var distance = 0f;
        var trainStore = world.GetStore<TrainMovementComponent>();
        if (trainStore.Count > 0) 
        {
            distance = trainStore.GetByIndex(0).DistanceTraveled;
        }

        foreach (var entity in world.Query<RenderableComponent, WagonSlotComponent>())
        {
            ref var render = ref world.Get<RenderableComponent>(entity);
            ref readonly var slot = ref world.Get<WagonSlotComponent>(entity);

            if (render.Node == null)
            {
                var node = BuildSafeWagon(entity, render);
                _trainRoot.AddChild(node);
                render.Node = node;
            }

            var targetPos = TrainLayout.GetLocalPosition(slot.SlotIndex, slot.Layer);
            
            // Apply visual bobbing effect based on train progress.
            var bobbing = Mathf.Sin(distance * 0.5f + slot.SlotIndex) * 0.1f;
            render.Node.Position = targetPos + Vector3.Up * bobbing;
        }
    }

    /// <summary>
    /// Programmatically constructs the Godot Node3D structure for a wagon.
    /// Includes the mesh, labels, and collision areas for interaction.
    /// </summary>
    private static Node3D BuildSafeWagon(Entity entity, RenderableComponent r)
    {
        var root = new Node3D { Name = $"Wagon_{entity.Id}" };

        // Main wagon body.
        var mesh = new MeshInstance3D { Name = "Body" };
        mesh.Mesh = new BoxMesh { 
            Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth) 
        };
        
        var mat = new StandardMaterial3D { AlbedoColor = r.Tint };
        mesh.SetSurfaceOverrideMaterial(0, mat);
        root.AddChild(mesh);

        // Debug/Information label floating above the wagon.
        var label3D = new Label3D
        {
            Text = r.Label,
            FontSize = 24,
            Position = new Vector3(0, 2.5f, 0),
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled
        };
        root.AddChild(label3D);
        
        // Interaction area for clicking or targeting.
        var area = new Area3D();
        var collision = new CollisionShape3D();
        collision.Shape = new BoxShape3D { 
            Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth) 
        };
        area.AddChild(collision);
        area.SetMeta("EntityId", entity.Id);
        root.AddChild(area);
        
        return root;
    }

    /// <summary>
    /// Event handler to clean up Godot nodes when an entity is removed from the ECS world.
    /// </summary>
    private void OnEntityDestroyed(Entity entity)
    {
        _trainRoot.GetNodeOrNull<Node3D>($"Wagon_{entity.Id}")
            .ToOption()
            .Match(node => node.QueueFree(), () => { });
    }
}