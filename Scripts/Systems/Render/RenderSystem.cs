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
public class RenderSystem : ISystem {
    /// <summary>
    /// The root Node3D for all train wagons in the rendering system.
    /// This node serves as the parent container for dynamically created wagon nodes,
    /// enabling hierarchical organization and easier management of visual elements
    /// associated with the train system.
    /// </summary>
    private readonly Node3D _trainRoot;

    /// <summary>
    /// Represents a system responsible for managing the rendering logic of entities
    /// within the context of a 3D scene.
    /// </summary>
    public RenderSystem(Node3D trainRoot, World world) {
        _trainRoot = trainRoot;
        world.OnEntityDestroyed += OnEntityDestroyed;
    }

    /// <summary>
    /// Updates the rendering state of all entities within the system by applying necessary transformations
    /// and visual effects based on the current world state and elapsed time.
    /// </summary>
    /// <param name="world">The game world containing the entities and their respective components.</param>
    /// <param name="delta">The amount of time elapsed since the last update, typically used for animations or movement calculations.</param>
    public void Update(World world, double delta) {
        float distance = 0f;
        ComponentStore<TrainMovementComponent> trainStore = world.GetStore<TrainMovementComponent>();
        if (trainStore.Count > 0) { distance = trainStore.GetByIndex(0).DistanceTraveled; }

        foreach (Entity entity in world.Query<RenderableComponent, WagonSlotComponent>()) {
            ref RenderableComponent render = ref world.Get<RenderableComponent>(entity);
            ref readonly WagonSlotComponent slot = ref world.Get<WagonSlotComponent>(entity);

            if (render.Node == null) {
                Node3D node = BuildSafeWagon(entity, render);
                _trainRoot.AddChild(node);
                render.Node = node;
            }

            Vector3 targetPosition = TrainLayout.GetLocalPosition(slot.SlotIndex, slot.Layer);

            float bobbing = Mathf.Sin(distance * 0.5f + slot.SlotIndex) * 0.1f;
            render.Node.Position = targetPosition + Vector3.Up * bobbing;
        }
    }

    /// <summary>
    /// Builds a safe and interactive 3D wagon node for rendering within a train system.
    /// </summary>
    /// <param name="entity">The entity associated with the wagon, providing a unique identifier.</param>
    /// <param name="renderable">The renderable component containing visual properties such as tint color and label.</param>
    /// <returns>A Node3D representing the fully constructed wagon, including its visual mesh, label, and interaction area.</returns>
    private static Node3D BuildSafeWagon(Entity entity, RenderableComponent renderable) {
        Node3D root = new() { Name = $"Wagon_{entity.Id}" };
        MeshInstance3D mesh = new() {
            Name = "Body",
            Mesh = new BoxMesh {
                Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth)
            }
        };

        StandardMaterial3D material = new() { AlbedoColor = renderable.Tint };
        mesh.SetSurfaceOverrideMaterial(0, material);
        root.AddChild(mesh);
        Label3D label3D = new() {
            Text = renderable.Label,
            FontSize = 24,
            Position = new Vector3(0, 2.5f, 0),
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled
        };
        root.AddChild(label3D);
        Area3D area = new();
        CollisionShape3D collision = new() {
            Shape = new BoxShape3D {
                Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth)
            }
        };
        area.AddChild(collision);
        area.SetMeta("EntityId", entity.Id);
        root.AddChild(area);

        return root;
    }

    /// <summary>
    /// Handles the destruction of an entity by removing its associated physical representation
    /// from the rendering system if one exists.
    /// </summary>
    /// <param name="entity">The entity that has been destroyed.</param>
    private void OnEntityDestroyed(Entity entity) =>
        _trainRoot.GetNodeOrNull<Node3D>($"Wagon_{entity.Id}")
            .ToOption()
            .Match(node => node.QueueFree(), () => { });
}
