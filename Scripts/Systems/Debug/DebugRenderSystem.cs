using Godot;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Debug;

/// <summary>
/// The DebugRenderSystem class is responsible for rendering debug visuals
/// within the scene. This includes drawing meshes and lines that aid in
/// visualizing entities, components, and relationships within the game world.
/// </summary>
public class DebugRenderSystem : ISystem {
    /// <summary>
    /// Represents an <see cref="ImmediateMesh"/> instance used for rendering debug visualization in the
    /// <see cref="DebugRenderSystem"/>. It allows dynamic construction of 3D geometry such as lines
    /// and circles for displaying entities' spatial attributes (e.g., attack ranges or positions) during runtime.
    /// </summary>
    private readonly ImmediateMesh _immediateMesh;

    /// <summary>
    /// Represents the root node of a train, serving as the anchor point for global positioning
    /// calculations and visual debug rendering within the DebugRenderSystem.
    /// </summary>
    private readonly Node3D _trainRoot;

    /// <summary>
    /// Represents a system for rendering debug visual elements, such as shapes, in a 3D environment.
    /// This system leverages ImmediateMesh for immediate mode rendering and configures the
    /// necessary materials and meshes for unshaded debug visualization.
    /// </summary>
    /// <param name="trainRoot">The root node of the train entity in the 3D scene.
    /// It is used as the parent node for adding the debug mesh instance to the current scene.</param>
    public DebugRenderSystem(Node3D trainRoot) {
        _trainRoot = trainRoot;
        _immediateMesh = new ImmediateMesh();

        StandardMaterial3D material = new() {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            VertexColorUseAsAlbedo = true
        };

        MeshInstance3D debugMeshInstance = new() {
            Mesh = _immediateMesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        };

        trainRoot.GetTree().CurrentScene.AddChild(debugMeshInstance);
    }

    /// <summary>
    /// Updates the debug rendering system by iterating through relevant entities and
    /// drawing visual debug representations such as circles in the 3D world.
    /// This method uses immediate mode rendering for debugging purposes.
    /// </summary>
    /// <param name="world">The ECS world instance containing entities and components.</param>
    /// <param name="delta">The elapsed time since the last frame, used for time-based calculations.</param>
    public void Update(World world, double delta) {
        _immediateMesh.ClearSurfaces();
        _immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        foreach (Entity entity in world.Query<EnemyComponent, PositionComponent>()) {
            ref readonly EnemyComponent enemy = ref world.Get<EnemyComponent>(entity);
            ref readonly PositionComponent positionComponent = ref world.Get<PositionComponent>(entity);

            DrawCircle3D(positionComponent.Value, enemy.AttackRange, new Color(1f, 0f, 0f, 0.5f));


            if (enemy.CurrentTarget.IsNone) { continue; }

            Entity target = enemy.CurrentTarget.Unwrap();
            if (!world.IsAlive(target)) { continue; }

            Option<WagonSlotComponent> slotOption = world.GetOptional<WagonSlotComponent>(target);
            if (slotOption.IsNone) { continue; }

            WagonSlotComponent wagonSlotComponent = slotOption.Unwrap();
            Vector3 targetGlobalPosition = _trainRoot.ToGlobal(
                TrainLayout.GetLocalPosition(wagonSlotComponent.SlotIndex, wagonSlotComponent.Layer) + new Vector3(0, 2f, 0)
            );
            DrawLine3D(positionComponent.Value, targetGlobalPosition, new Color(1f, 0.5f, 0f, 0.8f));
        }

        foreach (Entity entity in world.Query<WagonSlotComponent, TurretComponent>()) {
            ref readonly WagonSlotComponent wagonSlotComponent = ref world.Get<WagonSlotComponent>(entity);
            ref readonly TurretComponent weapon = ref world.Get<TurretComponent>(entity);

            Vector3 wagonGlobalPosition = _trainRoot.ToGlobal(
                TrainLayout.GetLocalPosition(wagonSlotComponent.SlotIndex, wagonSlotComponent.Layer) + new Vector3(0, 2f, 0)
            );
            DrawCircle3D(wagonGlobalPosition, weapon.Range, new Color(0f, 0.5f, 1f, 0.5f));
        }

        _immediateMesh.SurfaceEnd();
    }

    /// <summary>
    /// Draws a straight line in a 3D scene between two points with a specified color.
    /// </summary>
    /// <param name="start">The starting position of the line in 3D space.</param>
    /// <param name="end">The ending position of the line in 3D space.</param>
    /// <param name="color">The color to use when rendering the line.</param>
    private void DrawLine3D(Vector3 start, Vector3 end, Color color) {
        _immediateMesh.SurfaceSetColor(color);
        _immediateMesh.SurfaceAddVertex(start + Vector3.Up * 0.5f);
        _immediateMesh.SurfaceSetColor(color);
        _immediateMesh.SurfaceAddVertex(end + Vector3.Up * 0.5f);
    }

    /// <summary>
    /// Draws a 3D wireframe circle at the specified position, using the given radius and color.
    /// The circle is rendered in the XZ plane with a slight Y-axis offset for visibility.
    /// </summary>
    /// <param name="center">The center position of the circle in 3D space.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color used to render the circle.</param>
    private void DrawCircle3D(Vector3 center, float radius, Color color) {
        const int segments = 32;
        for (int segmentIndex = 0; segmentIndex < segments; segmentIndex++) {
            float angle1 = segmentIndex / (float)segments * Mathf.Tau;
            float angle2 = (segmentIndex + 1) / (float)segments * Mathf.Tau;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, 0.2f, Mathf.Sin(angle1) * radius);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * radius, 0.2f, Mathf.Sin(angle2) * radius);

            _immediateMesh.SurfaceSetColor(color);
            _immediateMesh.SurfaceAddVertex(point1);
            _immediateMesh.SurfaceSetColor(color);
            _immediateMesh.SurfaceAddVertex(point2);
        }
    }
}
