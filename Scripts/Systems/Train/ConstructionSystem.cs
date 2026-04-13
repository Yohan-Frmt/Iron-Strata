using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// System that handles the construction of new wagons and upgrading existing ones.
/// It manages the interaction between the player's mouse and the 3D train model.
/// </summary>
public class ConstructionSystem : ISystem
{
    private readonly Camera3D _camera;
    private readonly MeshInstance3D _previewGhost;
    private readonly StandardMaterial3D _previewMat;
    private readonly Control _bottomHud;
    private readonly Node3D _trainRoot;

    /// <summary>
    /// Initializes a new instance of the ConstructionSystem.
    /// </summary>
    public ConstructionSystem(Camera3D camera, MeshInstance3D previewGhost, Control bottomHud, Node3D trainRoot)
    {
        _camera = camera;
        _previewGhost = previewGhost;
        _bottomHud = bottomHud;
        _trainRoot = trainRoot;
        _previewMat = (StandardMaterial3D)_previewGhost.Mesh.SurfaceGetMaterial(0);
    }

    /// <summary>
    /// Updates the visibility of construction-related UI and previews.
    /// </summary>
    public void Update(World world, double delta)
    {
        world.Query<LocationComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<LocationComponent>(e))
            .Match(loc => 
            {
                // Show/hide the construction HUD based on whether the train is in transit.
                if (_bottomHud != null) _bottomHud.Visible = loc.IsInTransit;
                
                // Hide the placement preview if not in transit.
                if (!loc.IsInTransit && _previewGhost.Visible) 
                    _previewGhost.Visible = false;
            }, () => { });
    }

    /// <summary>
    /// Hides the wagon placement preview ghost.
    /// </summary>
    public void HidePreview()
    {
        if (_previewGhost != null) _previewGhost.Visible = false;
    }

    /// <summary>
    /// Updates the position and color of the placement preview based on mouse position.
    /// Uses raycasting to detect existing wagons or the ground.
    /// </summary>
    public void UpdatePreview(World world, WagonType cardType, Vector2 mousePos)
    {
        var locOption = world.Query<LocationComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<LocationComponent>(e));

        if (locOption.Match(l => !l.IsInTransit, () => true)) return;

        // Perform raycast from camera to world.
        var spaceState = _bottomHud.GetViewport().World3D.DirectSpaceState;
        var rayOrigin = _camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + _camera.ProjectRayNormal(mousePos) * 1000f;
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithAreas = true;

        var result = spaceState.IntersectRay(query);

        if (result.Count <= 0) return;

        var collider = (Node3D)result["collider"];

        // Check if we hit an existing wagon.
        if (collider.HasMeta("EntityId"))
        {
            var entityId = (int)collider.GetMeta("EntityId");
            var hitEntity = new Entity(entityId);

            if (world.IsAlive(hitEntity))
            {
                var hitSlot = world.Get<WagonSlotComponent>(hitEntity);
                var highestLayer = -1;
                var topType = WagonType.Locomotive;

                // Find the topmost wagon at this slot index.
                foreach (var e in world.Query<WagonSlotComponent, WagonTypeComponent>())
                {
                    var s = world.Get<WagonSlotComponent>(e);
                    if (s.SlotIndex != hitSlot.SlotIndex || s.Layer <= highestLayer) continue;
                    highestLayer = s.Layer;
                    topType = world.Get<WagonTypeComponent>(e).Type;
                }

                // Cannot build on top of the locomotive.
                if (topType == WagonType.Locomotive) 
                { 
                    _previewGhost.Visible = false; 
                    return; 
                }

                // If types match, it's an upgrade (yellow highlight).
                if (topType == cardType)
                {
                    _previewMat.AlbedoColor = new Color(1.0f, 0.8f, 0.2f, 0.6f);
                    _previewMat.Emission = new Color(1.0f, 0.8f, 0.2f);
                    _previewGhost.Position = TrainLayout.GetLocalPosition(hitSlot.SlotIndex, highestLayer);
                    _previewGhost.Scale = new Vector3(1.1f, 1.1f, 1.1f);
                }
                // Otherwise, it's a new layer (blue highlight).
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
        // Check if we hit the floor to append a new wagon at the end of the train.
        else if (collider.Name == "FloorBody")
        {
            var hitPoint = (Vector3)result["position"];
            var localHit = _trainRoot.ToLocal(hitPoint);
            var maxSlot = world.Query<WagonSlotComponent>()
                .Select(e => world.Get<WagonSlotComponent>(e).SlotIndex)
                .Prepend(0).Max();
            
            var lastWagonX = -maxSlot * (TrainLayout.WagonLength + TrainLayout.WagonGap);
            
            // If mouse is behind the last wagon, show placement ghost (green highlight).
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

    /// <summary>
    /// Attempts to play a card (build or upgrade a wagon) at the current mouse position.
    /// </summary>
    /// <returns>True if the card was successfully played, false otherwise.</returns>
    public bool TryPlayCard(World world, WagonType cardType, int cost, Vector2 mousePos)
    {
        var locOption = world.Query<LocationComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<LocationComponent>(e));

        if (locOption.Match(l => !l.IsInTransit, () => true)) return false;

        // Check if player has enough resources.
        var resOption = world.Query<ResourceComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<ResourceComponent>(e));

        if (resOption.Match(r => r.Scrap < cost, () => true)) return false;
        var resources = resOption.Unwrap();

        var spaceState = _bottomHud.GetViewport().World3D.DirectSpaceState;
        var rayOrigin = _camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + _camera.ProjectRayNormal(mousePos) * 1000f;
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithAreas = true;
        var result = spaceState.IntersectRay(query);

        if (result.Count <= 0) return false;
        var collider = (Node3D)result["collider"];

        // Handle hitting an existing wagon.
        if (collider.HasMeta("EntityId"))
        {
            var entityId = (int)collider.GetMeta("EntityId");
            ApplyCardToWagon(world, new Entity(entityId), cardType);
            resources.Scrap -= cost;
            return true;
        }
        
        // Handle hitting the ground to append a new wagon.
        if (collider.Name != "FloorBody") return false;
        
        var hitPoint = (Vector3)result["position"];
        var localHit = _trainRoot.ToLocal(hitPoint);
        var maxSlot = world.Query<WagonSlotComponent>()
            .Select(e => world.Get<WagonSlotComponent>(e).SlotIndex)
            .Prepend(0).Max();
        
        var lastWagonX = -maxSlot * (TrainLayout.WagonLength + TrainLayout.WagonGap);
        if (!(localHit.X < lastWagonX + 2f)) return false;
        
        CreateNewWagon(world, maxSlot + 1, 0, cardType);
        resources.Scrap -= cost;
        return true;
    }

    /// <summary>
    /// Applies a card's effect to an existing wagon stack.
    /// If the types match, it upgrades the top wagon; otherwise, it builds a new one on top.
    /// </summary>
    private static void ApplyCardToWagon(World world, Entity hitEntity, WagonType cardType)
    {
        if (!world.IsAlive(hitEntity)) return;
        var hitSlot = world.Get<WagonSlotComponent>(hitEntity);
        var topEntity = hitEntity;
        var highestLayer = -1;

        // Find the topmost wagon in the stack.
        foreach (var e in world.Query<WagonSlotComponent, WagonTypeComponent>())
        {
            var s = world.Get<WagonSlotComponent>(e);
            if (s.SlotIndex != hitSlot.SlotIndex || s.Layer <= highestLayer) continue;
            highestLayer = s.Layer;
            topEntity = e;
        }

        var topType = world.Get<WagonTypeComponent>(topEntity).Type;
        if (topType == WagonType.Locomotive) return;

        // Upgrade if types match.
        if (topType == cardType)
        {
            var health = world.Get<HealthComponent>(topEntity);
            health.Max += 50f;
            health.Current += 50f;
            
            // Special case for combat wagon upgrades.
            if (cardType == WagonType.Combat && world.Has<TurretComponent>(topEntity))
            {
                var turret = world.Get<TurretComponent>(topEntity);
                turret.Damage += 10f;
                turret.FireRate += 3f;
            }
        }
        // Build new layer if types differ.
        else
        {
            CreateNewWagon(world, hitSlot.SlotIndex, highestLayer + 1, cardType);
        }
    }

    /// <summary>
    /// Creates a new wagon entity with all necessary components and adds it to the ECS world.
    /// </summary>
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

        // Add specific components based on wagon type.
        if (type == WagonType.Combat)
            world.Add(entity, new TurretComponent { Range = 35f, Damage = 15f, FireRate = 6f });
    }
}