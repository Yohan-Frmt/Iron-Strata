using Godot;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Components.Shared;

namespace IronStrata.Scripts.Systems.Debug;

public class DebugRenderSystem : ISystem
{
    private readonly ImmediateMesh _immediateMesh;
    private readonly Node3D _trainRoot;

    public DebugRenderSystem(Node3D trainRoot)
    {
        _trainRoot = trainRoot;
        _immediateMesh = new ImmediateMesh();
        var material = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            VertexColorUseAsAlbedo = true
        };

        var debugMeshInstance = new MeshInstance3D
        {
            Mesh = _immediateMesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        };

        trainRoot.GetTree().CurrentScene.AddChild(debugMeshInstance);
    }

    public void Update(World world, double delta)
    {
        _immediateMesh.ClearSurfaces();
        _immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        foreach (var entity in world.Query<EnemyComponent, PositionComponent>())
        {
            var enemy = world.Get<EnemyComponent>(entity);
            var pos = world.Get<PositionComponent>(entity);
            DrawCircle3D(pos.Value, enemy.AttackRange, new Color(1f, 0f, 0f, 0.5f));
            if (!world.IsAlive(enemy.CurrentTarget)) continue;
            var slotComp = world.Get<WagonSlotComponent>(enemy.CurrentTarget);
            const float wagonSize = 5f;
            var targetLocalPos = new Vector3(-slotComp.SlotIndex * wagonSize, 0, 0);
            var targetGlobalPos = _trainRoot.GlobalPosition + targetLocalPos;
            DrawLine3D(pos.Value, targetGlobalPos, new Color(1f, 0.5f, 0f, 0.8f)); // Ligne orange
        }

        foreach (var entity in world.Query<WagonSlotComponent, TurretComponent>())
        {
            var slotComp = world.Get<WagonSlotComponent>(entity);
            var weapon = world.Get<TurretComponent>(entity);
            var targetLocalPos = new Vector3(-slotComp.SlotIndex * 5f, 0, 0);
            var wagonGlobalPos = _trainRoot.GlobalPosition + targetLocalPos;
            DrawCircle3D(wagonGlobalPos, weapon.Range, new Color(0f, 0.5f, 1f, 0.5f));
        }


        _immediateMesh.SurfaceEnd();
    }

    private void DrawLine3D(Vector3 start, Vector3 end, Color color)
    {
        _immediateMesh.SurfaceSetColor(color);
        _immediateMesh.SurfaceAddVertex(start + Vector3.Up * 0.5f);
        _immediateMesh.SurfaceSetColor(color);
        _immediateMesh.SurfaceAddVertex(end + Vector3.Up * 0.5f);
    }

    private void DrawCircle3D(Vector3 center, float radius, Color color)
    {
        const int segments = 32;
        for (var i = 0; i < segments; i++)
        {
            var angle1 = i / (float)segments * Mathf.Tau;
            var angle2 = (i + 1) / (float)segments * Mathf.Tau;
            var p1 = center + new Vector3(Mathf.Cos(angle1) * radius, 0.2f, Mathf.Sin(angle1) * radius);
            var p2 = center + new Vector3(Mathf.Cos(angle2) * radius, 0.2f, Mathf.Sin(angle2) * radius);
            _immediateMesh.SurfaceSetColor(color);
            _immediateMesh.SurfaceAddVertex(p1);
            _immediateMesh.SurfaceSetColor(color);
            _immediateMesh.SurfaceAddVertex(p2);
        }
    }
}