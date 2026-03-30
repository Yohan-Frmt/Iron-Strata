using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Train;

public class ConstructionSystem : ISystem
{
    private readonly Camera3D _camera;
    private readonly MeshInstance3D _previewGhost;
    private readonly StandardMaterial3D _previewMat;
    private readonly Control _bottomHud;
    private readonly Node3D _trainRoot;

    public ConstructionSystem(Camera3D camera, MeshInstance3D previewGhost, Control bottomHud, Node3D trainRoot)
    {
        _camera = camera;
        _previewGhost = previewGhost;
        _bottomHud = bottomHud;
        _trainRoot = trainRoot;
        _previewMat = (StandardMaterial3D)_previewGhost.Mesh.SurfaceGetMaterial(0);
    }

    public void Update(World world, double delta)
    {
        var locEntity = world.Query<LocationComponent>().FirstOrDefault();
        if (locEntity == null) return;
        var loc = world.Get<LocationComponent>(locEntity);
        if (_bottomHud != null) _bottomHud.Visible = loc.IsInTransit;
        if (!loc.IsInTransit && _previewGhost.Visible) _previewGhost.Visible = false;
    }

    public void HidePreview()
    {
        if (_previewGhost != null) _previewGhost.Visible = false;
    }

    public void UpdatePreview(World world, WagonType cardType, Vector2 mousePos)
    {
        var loc = world.Query<LocationComponent>().Select(e => world.Get<LocationComponent>(e)).FirstOrDefault();
        if (loc is not { IsInTransit: true }) return;

        var spaceState = _bottomHud.GetViewport().World3D.DirectSpaceState;
        var rayOrigin = _camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + _camera.ProjectRayNormal(mousePos) * 1000f;
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithAreas = true;

        var result = spaceState.IntersectRay(query);

        if (result.Count <= 0) return;

        var collider = (Node3D)result["collider"];

        if (collider.HasMeta("EntityId"))
        {
            var entityId = (int)collider.GetMeta("EntityId");
            var hitEntity = new Entity(entityId);

            if (world.IsAlive(hitEntity))
            {
                var hitSlot = world.Get<WagonSlotComponent>(hitEntity);
                var highestLayer = -1;
                var topType = WagonType.Locomotive;

                foreach (var e in world.Query<WagonSlotComponent, WagonTypeComponent>())
                {
                    var s = world.Get<WagonSlotComponent>(e);
                    if (s.SlotIndex != hitSlot.SlotIndex || s.Layer <= highestLayer) continue;
                    highestLayer = s.Layer;
                    topType = world.Get<WagonTypeComponent>(e).Type;
                }

                if (topType == WagonType.Locomotive) { _previewGhost.Visible = false; return; }

                if (topType == cardType)
                {
                    _previewMat.AlbedoColor = new Color(1.0f, 0.8f, 0.2f, 0.6f);
                    _previewMat.Emission = new Color(1.0f, 0.8f, 0.2f);
                    _previewGhost.Position = TrainLayout.GetLocalPosition(hitSlot.SlotIndex, highestLayer);
                    _previewGhost.Scale = new Vector3(1.1f, 1.1f, 1.1f);
                }
                else
                {
                    _previewMat.AlbedoColor = new Color(0.2f, 0.6f, 1.0f, 0.6f);
                    _previewMat.Emission = new Color(0.2f, 0.6f, 1.0f);
                    _previewGhost.Position = TrainLayout.GetLocalPosition(hitSlot.SlotIndex, highestLayer + 1);
                    _previewGhost.Scale = Vector3.One;
                }
                _previewGhost.Visible = true;
                return;
            }
        }
        else if (collider.Name == "FloorBody")
        {
            var hitPoint = (Vector3)result["position"];
            var localHit = _trainRoot.ToLocal(hitPoint);
            var maxSlot = world.Query<WagonSlotComponent>().Select(e => world.Get<WagonSlotComponent>(e).SlotIndex).Prepend(0).Max();
            var lastWagonX = -maxSlot * (TrainLayout.WagonLength + TrainLayout.WagonGap);
            if (localHit.X < lastWagonX + 2f)
            {
                _previewMat.AlbedoColor = new Color(0.2f, 1.0f, 0.2f, 0.6f);
                _previewMat.Emission = new Color(0.2f, 1.0f, 0.2f);
                _previewGhost.Position = TrainLayout.GetLocalPosition(maxSlot + 1, 0);
                _previewGhost.Scale = Vector3.One;
                _previewGhost.Visible = true;
                return;
            }
        }
        _previewGhost.Visible = false;
    }

    public bool TryPlayCard(World world, WagonType cardType, int cost, Vector2 mousePos)
    {
        var loc = world.Query<LocationComponent>().Select(e => world.Get<LocationComponent>(e)).FirstOrDefault();
        if (loc is not { IsInTransit: true }) return false;

        var resEntity = world.Query<ResourceComponent>().FirstOrDefault();
        if (resEntity == null) return false;
        var resources = world.Get<ResourceComponent>(resEntity);
        if (resources.Scrap < cost) return false;

        var spaceState = _bottomHud.GetViewport().World3D.DirectSpaceState;
        var rayOrigin = _camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + _camera.ProjectRayNormal(mousePos) * 1000f;
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithAreas = true;
        var result = spaceState.IntersectRay(query);

        if (result.Count <= 0) return false;
        var collider = (Node3D)result["collider"];

        if (collider.HasMeta("EntityId"))
        {
            var entityId = (int)collider.GetMeta("EntityId");
            ApplyCardToWagon(world, new Entity(entityId), cardType);
            resources.Scrap -= cost;
            return true;
        }
        if (collider.Name != "FloorBody") return false;
        var hitPoint = (Vector3)result["position"];
        var localHit = _trainRoot.ToLocal(hitPoint);
        var maxSlot = world.Query<WagonSlotComponent>().Select(e => world.Get<WagonSlotComponent>(e).SlotIndex).Prepend(0).Max();
        var lastWagonX = -maxSlot * (TrainLayout.WagonLength + TrainLayout.WagonGap);
        if (!(localHit.X < lastWagonX + 2f)) return false;
        CreateNewWagon(world, maxSlot + 1, 0, cardType);
        resources.Scrap -= cost;
        return true;
    }

    private static void ApplyCardToWagon(World world, Entity hitEntity, WagonType cardType)
    {
        if (!world.IsAlive(hitEntity)) return;
        var hitSlot = world.Get<WagonSlotComponent>(hitEntity);
        var topEntity = hitEntity;
        var highestLayer = -1;

        foreach (var e in world.Query<WagonSlotComponent, WagonTypeComponent>())
        {
            var s = world.Get<WagonSlotComponent>(e);
            if (s.SlotIndex != hitSlot.SlotIndex || s.Layer <= highestLayer) continue;
            highestLayer = s.Layer;
            topEntity = e;
        }

        var topType = world.Get<WagonTypeComponent>(topEntity).Type;
        if (topType == WagonType.Locomotive) return;

        if (topType == cardType)
        {
            var health = world.Get<HealthComponent>(topEntity);
            health.Max += 50f;
            health.Current += 50f;
            if (cardType == WagonType.Combat && world.Has<TurretComponent>(topEntity))
            {
                var turret = world.Get<TurretComponent>(topEntity);
                turret.Damage += 10f;
                turret.FireRate += 3f;
            }
        }
        else
        {
            CreateNewWagon(world, hitSlot.SlotIndex, highestLayer + 1, cardType);
        }
    }

    private static void CreateNewWagon(World world, int slot, int layer, WagonType type)
    {
        var tint = type switch
        {
            WagonType.Combat => TrainLayout.ColorCombat,
            WagonType.Living => TrainLayout.ColorLiving,
            WagonType.Storage => TrainLayout.ColorStorage,
            WagonType.Research => TrainLayout.ColorResearch,
            _ => Colors.Gray
        };

        var entity = world.CreateEntity();
        world.Add(entity, new WagonTypeComponent { Type = type, BlueprintId = "card_spawn" });
        world.Add(entity, new WagonSlotComponent { SlotIndex = slot, Layer = layer });
        world.Add(entity, new HealthComponent { Max = 150f, Current = 150f });
        world.Add(entity, new RenderableComponent { Tint = tint, Label = type.ToString().ToUpper() });

        if (type == WagonType.Combat)
            world.Add(entity, new TurretComponent { Range = 35f, Damage = 15f, FireRate = 6f });
    }
}