using System.Linq;
using Godot;
using Godot.Collections;
using IronStrata.Scripts.Components.Camera;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// Represents a configuration for visualizing a preview object within the game.
/// It defines the position, scale, colors, and visibility state of the preview object.
/// </summary>
internal struct PreviewConfig
{
    public Vector3 Position { get; init; }
    public Vector3 Scale { get; init; }
    public Color AlbedoColor { get; init; }
    public Color Emission { get; init; }
    public bool Visible { get; init; }
}

/// <summary>
/// System that handles the construction of new wagons and upgrading existing ones.
/// It manages the interaction between the player's mouse and the 3D train model.
/// </summary>
public class ConstructionSystem : ISystem
{
    private readonly MeshInstance3D _previewGhost;
    private readonly StandardMaterial3D _previewMat;
    private readonly Control _bottomHud;
    private readonly Node3D _trainRoot;

    /// <summary>
    /// Initializes a new instance of the ConstructionSystem.
    /// </summary>
    public ConstructionSystem(MeshInstance3D previewGhost, Control bottomHud, Node3D trainRoot)
    {
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
            .Bind(world.GetOptional<LocationComponent>)
            .Match(loc =>
                {
                    if (_bottomHud != null) _bottomHud.Visible = loc.IsInTransit;
                    if (!loc.IsInTransit && _previewGhost.Visible) _previewGhost.Visible = false;
                }
            );
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
        var shouldExit = world.Query<LocationComponent>()
            .FirstOptional()
            .Bind(world.GetOptional<LocationComponent>)
            .Match(
                l => !l.IsInTransit,
                () => true
            );

        if (shouldExit)
        {
            _previewGhost.Visible = false;
            return;
        }

        world.Query<CameraComponent>()
            .FirstOptional()
            .Bind(world.GetOptional<CameraComponent>)
            .Match(cam =>
            {
                PerformRaycast(cam.Camera, mousePos)
                    .Bind(GetColliderData)
                    .Bind(collider => ProcessCollision(world, collider.Collider, collider.Position, cardType))
                    .Match(
                        ApplyPreviewConfig,
                        () => _previewGhost.Visible = false
                    );
            });
    }

    /// <summary>
    /// Performs a raycast from the camera to the mouse position.
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="mousePos"></param>
    /// <returns></returns>
    private Option<Dictionary> PerformRaycast(Camera3D camera, Vector2 mousePos)
    {
        var spaceState = _bottomHud.GetViewport().World3D.DirectSpaceState;
        var rayOrigin = camera.ProjectRayOrigin(mousePos);
        var rayEnd = rayOrigin + camera.ProjectRayNormal(mousePos) * 1000f;
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithAreas = true;

        var result = spaceState.IntersectRay(query);
        return result.Count > 0
            ? Option<Dictionary>.Some(result)
            : Option<Dictionary>.None;
    }

    /// <summary>
    /// Applies the specified preview configuration to the preview object.
    /// </summary>
    /// <param name="config">The configuration to apply to the preview object.</param>
    private void ApplyPreviewConfig(PreviewConfig config)
    {
        _previewGhost.Position = config.Position;
        _previewGhost.Scale = config.Scale;
        _previewMat.AlbedoColor = config.AlbedoColor;
        _previewMat.Emission = config.Emission;
        _previewGhost.Visible = config.Visible;
    }


    /// <summary>
    /// Attempts to play a card (build or upgrade a wagon) at the current mouse position.
    /// </summary>
    /// <returns>True if the card was successfully played, false otherwise.</returns>
    public Result<bool, string> TryPlayCard(World world, WagonType cardType, int cost, Vector2 mousePos)
    {
        var resOption = world.Query<ResourceComponent>().FirstOptional().Bind(world.GetOptional<ResourceComponent>);
        if (resOption.Match(r => r.Scrap < cost, () => true)) 
            return Result.Err<bool, string>("Not enough scrap!");
        
        return world.Query<CameraComponent>()
            .FirstOptional()
            .Bind(world.GetOptional<CameraComponent>)
            .Match(
                cam => ExecutePlacement(world, cam.Camera, cardType, cost, mousePos, resOption.Unwrap()),
                () => Result.Err<bool, string>("Camera not found")
            );
    }

    /// <summary>
    /// Executes the placement of a new wagon or updates an existing one based on the provided parameters.
    /// </summary>
    /// <param name="world">The ECS world instance containing all entities and components.</param>
    /// <param name="camera">The 3D camera used for raycasting to determine placement location.</param>
    /// <param name="type">The type of wagon to place or update.</param>
    /// <param name="cost">The resource cost for the placement action.</param>
    /// <param name="mousePos">The mouse position in screen space used for raycasting.</param>
    /// <param name="resources">The resource component containing information on available resources.</param>
    /// <returns>
    /// A Result object containing a boolean indicating success or failure, or an error message describing the reason for failure.
    /// </returns>
    private Result<bool, string> ExecutePlacement(World world, Camera3D camera, WagonType type, int cost, Vector2 mousePos, ResourceComponent resources)
    {
        return PerformRaycast(camera, mousePos)
            .Bind(GetColliderData)
            .Match(
                data => 
                {
                    if (data.Collider.HasMeta("EntityId"))
                    {
                        var entityId = (int)data.Collider.GetMeta("EntityId");
                        ApplyCardToWagon(world, new Entity(entityId), type);
                        resources.Scrap -= cost;
                        return Result.Ok<bool, string>(true);
                    }

                    if (data.Collider.Name == "FloorBody" && IsValidFloorSpace(world, data.Position))
                    {
                        var maxSlot = GetMaxSlot(world);
                        CreateNewWagon(world, maxSlot + 1, 0, type);
                        resources.Scrap -= cost;
                        return Result.Ok<bool, string>(true);
                    }

                    return Result.Err<bool, string>("Invalid placement area");
                },
                () => Result.Err<bool, string>("Nothing hit")
            );
    }

    /// <summary>
    /// Extracts collider and position data from the provided result dictionary.
    /// </summary>
    /// <param name="result">The dictionary containing raycast hit data, expected to have "collider" and "position" keys.</param>
    /// <returns>An <see cref="Option{T}"/> containing a tuple with the collided <see cref="Node3D"/> and its hit position
    /// as a <see cref="Vector3"/>, or <see cref="Option.None"/> if the required data is missing.</returns>
    private static Option<(Node3D Collider, Vector3 Position)> GetColliderData(Dictionary result)
    {
        if (result.TryGetValue("collider", out var col) && result.TryGetValue("position", out var pos))
            return Option<(Node3D, Vector3)>.Some(((Node3D)col, (Vector3)pos));
        return Option<(Node3D, Vector3)>.None;
    }

    /// <summary>
    /// Processes a collision to determine the appropriate preview configuration for placing or upgrading a wagon.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="collider">The node corresponding to the object that was collided with.</param>
    /// <param name="hitPos">The position of the collision in world coordinates.</param>
    /// <param name="cardType">The type of wagon card being processed.</param>
    /// <returns>An Option containing a <see cref="PreviewConfig"/> if a valid preview configuration can be generated; otherwise, None.</returns>
    private Option<PreviewConfig> ProcessCollision(World world, Node3D collider, Vector3 hitPos, WagonType cardType)
    {
        if (collider.HasMeta("EntityId"))
        {
            var entityId = (int)collider.GetMeta("EntityId");
            var hitEntity = new Entity(entityId);
            if (!world.IsAlive(hitEntity)) return Option<PreviewConfig>.None;

            var hitSlot = world.Get<WagonSlotComponent>(hitEntity);
            var (highestLayer, topType) = FindTopWagon(world, hitSlot.SlotIndex);

            if (topType == WagonType.Locomotive) return Option<PreviewConfig>.None;

            var isUpgrade = topType == cardType;
            return Option<PreviewConfig>.Some(new PreviewConfig {
                Position = TrainLayout.GetLocalPosition(hitSlot.SlotIndex, isUpgrade ? highestLayer : highestLayer + 1),
                Scale = isUpgrade ? new Vector3(1.1f, 1.1f, 1.1f) : Vector3.One,
                AlbedoColor = isUpgrade ? new Color(1, 0.8f, 0.2f, 0.6f) : new Color(0.2f, 0.6f, 1, 0.6f),
                Emission = isUpgrade ? new Color(1, 0.8f, 0.2f) : new Color(0.2f, 0.6f, 1),
                Visible = true
            });
        }

        if (collider.Name == "FloorBody" && IsValidFloorSpace(world, hitPos))
        {
            return Option<PreviewConfig>.Some(new PreviewConfig {
                Position = TrainLayout.GetLocalPosition(GetMaxSlot(world) + 1, 0),
                Scale = Vector3.One,
                AlbedoColor = new Color(0.2f, 1, 0.2f, 0.6f),
                Emission = new Color(0.2f, 1, 0.2f),
                Visible = true
            });
        }

        return Option<PreviewConfig>.None;
    }

    private bool IsValidFloorSpace(World world, Vector3 globalHitPos)
    {
        var localHit = _trainRoot.ToLocal(globalHitPos);
        var lastWagonX = -GetMaxSlot(world) * (TrainLayout.WagonLength + TrainLayout.WagonGap);
        return localHit.X < lastWagonX + 2f;
    }

    private static int GetMaxSlot(World world) => world.Query<WagonSlotComponent>()
        .Select(e => world.Get<WagonSlotComponent>(e).SlotIndex)
        .Prepend(0).Max();

    private static (int layer, WagonType type) FindTopWagon(World world, int slotIndex)
    {
        var wagons = world.Query<WagonSlotComponent, WagonTypeComponent>()
            .Select(e => (Slot: world.Get<WagonSlotComponent>(e), world.Get<WagonTypeComponent>(e).Type))
            .Where(w => w.Slot.SlotIndex == slotIndex)
            .OrderByDescending(w => w.Slot.Layer)
            .FirstOrDefault();
            
        return wagons.Slot != null ? (wagons.Slot.Layer, wagons.Type) : (-1, WagonType.Locomotive);
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

            if (cardType != WagonType.Combat || !world.Has<TurretComponent>(topEntity)) return;
            var turret = world.Get<TurretComponent>(topEntity);
            turret.Damage += 10f;
            turret.FireRate += 3f;
        }
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

        if (type == WagonType.Combat)
            world.Add(entity, new TurretComponent { Range = 35f, Damage = 15f, FireRate = 6f });
    }
}