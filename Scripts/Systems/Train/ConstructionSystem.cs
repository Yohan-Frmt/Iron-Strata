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
internal readonly struct PreviewConfig {
    /// <summary>
    /// Gets or sets the position of the preview object within the 3D space.
    /// This property defines the spatial location of the object and is integral
    /// to positioning the preview visualization accurately in the game world.
    /// </summary>
    public Vector3 Position { get; init; }

    /// <summary>
    /// Gets or sets the scale of the preview object in the 3D space.
    /// This property determines the proportional size of the object relative to its
    /// default dimensions, influencing how the object appears in the game world.
    /// </summary>
    public Vector3 Scale { get; init; }

    /// <summary>
    /// Gets or sets the albedo color of the preview object's material.
    /// This property defines the base color and transparency of the material,
    /// contributing to the visual appearance of the preview object in the scene.
    /// </summary>
    public Color AlbedoColor { get; init; }

    /// <summary>
    /// Gets or sets the emission color of the preview object.
    /// This property determines the light-emitting color for the material,
    /// allowing for visual effects such as glowing highlights or distinctive visual cues.
    /// </summary>
    public Color Emission { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the preview object is visible.
    /// This property determines the visibility state of the object within the game world,
    /// allowing toggling of its appearance during configuration or runtime operations.
    /// </summary>
    public bool Visible { get; init; }
}

/// <summary>
/// System that handles the construction of new wagons and upgrading existing ones.
/// It manages the interaction between the player's mouse and the 3D train model.
/// </summary>
public class ConstructionSystem : ISystem {
    private readonly MeshInstance3D _previewGhost;
    private readonly StandardMaterial3D _previewMat;
    private readonly Control _bottomHud;
    private readonly Node3D _trainRoot;

    /// <summary>
    /// Handles the construction-related functionalities within the train system,
    /// responsible for managing UI, previews, and material interactions.
    /// </summary>
    public ConstructionSystem(MeshInstance3D previewGhost, Control bottomHud, Node3D trainRoot) {
        _previewGhost = previewGhost;
        _bottomHud = bottomHud;
        _trainRoot = trainRoot;
        _previewMat = (StandardMaterial3D)_previewGhost.GetSurfaceOverrideMaterial(0);

        // Ensure preview ghost moves with the train
        if (_previewGhost.GetParent() != _trainRoot) {
            _previewGhost.GetParent()?.RemoveChild(_previewGhost);
            _trainRoot.AddChild(_previewGhost);
        }
    }

    /// <summary>
    /// Updates the construction system state, checking visibility interactions.
    /// </summary>
    /// <param name="world">The game world instance containing entities and their components.</param>
    /// <param name="delta">The time elapsed since the last update, used for timing calculations.</param>
    public void Update(World world, double delta) {
        // HUD and preview are always accessible — building is allowed at any time
    }

    /// <summary>
    /// Hides the construction preview ghost in the train system, disabling its visibility.
    /// This is used to remove any visual indicators for construction placement from the scene.
    /// </summary>
    public void HidePreview() {
        if (_previewGhost != null) { _previewGhost.Visible = false; }
    }

    /// <summary>
    /// Updates the visual preview of a wagon placement within the construction system, including
    /// raycasting to identify the appropriate location and applying the configuration for the
    /// preview object based on the current mouse position and selected wagon type.
    /// </summary>
    /// <param name="world">The game world instance, used to query entities and retrieve components related to the construction system.</param>
    /// <param name="cardType">The type of wagon being previewed, determining the configuration applied to the preview object.</param>
    /// <param name="mousePosition">The current position of the mouse in screen coordinates, used for detecting the intended placement location.</param>
    public void UpdatePreview(World world, WagonType cardType, Vector2 mousePosition) {
        Option<Entity> cameraEntityOption = world.QueryFirst<CameraComponent>();
        if (cameraEntityOption.IsNone) { return; }

        CameraComponent camera = world.Get<CameraComponent>(cameraEntityOption.Unwrap());
        PerformRaycast(camera.Camera, mousePosition)
            .Bind(GetColliderData)
            .Bind(collider => ProcessCollision(world, collider.Collider, collider.Position, cardType))
            .Match(
                ApplyPreviewConfig,
                () => _previewGhost.Visible = false
            );
    }

    /// <summary>
    /// Performs a raycast in the 3D space from a camera's perspective based on a given mouse position.
    /// This is used to detect objects or surfaces in the scene that the ray intersects with.
    /// </summary>
    /// <param name="camera">The camera from which the ray is projected.</param>
    /// <param name="mousePosition">The position of the mouse on the screen, used to determine the ray's direction.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing a dictionary with the raycast results if an intersection is found;
    /// otherwise, an <see cref="Option{T}.None"/> indicating no intersection occurred.
    /// </returns>
    private Option<Dictionary> PerformRaycast(Camera3D camera, Vector2 mousePosition) {
        PhysicsDirectSpaceState3D spaceState = _bottomHud.GetViewport().World3D.DirectSpaceState;
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayEnd = rayOrigin + camera.ProjectRayNormal(mousePosition) * 1000f;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithAreas = true;

        Dictionary result = spaceState.IntersectRay(query);
        return result.Count > 0
            ? Option<Dictionary>.Some(result)
            : Option<Dictionary>.None;
    }

    /// <summary>
    /// Applies the specified preview configuration to the 3D preview object,
    /// updating its position, scale, color, and visibility based on the given settings.
    /// </summary>
    /// <param name="config">The configuration to apply to the preview object.</param>
    private void ApplyPreviewConfig(PreviewConfig config) {
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
    public Result<bool, string> TryPlayCard(World world, WagonType cardType, int cost, Vector2 mousePos) {
        Option<Entity> resourceEntityOption = world.QueryFirst<ResourceComponent>();
        if (resourceEntityOption.IsNone) { return Result.Err<bool, string>("Resources not found!"); }

        ref ResourceComponent resources = ref world.Get<ResourceComponent>(resourceEntityOption.Unwrap());
        if (resources.Scrap < cost) { return Result.Err<bool, string>("Not enough scrap!"); }

        Option<Entity> cameraEntityOption = world.QueryFirst<CameraComponent>();
        if (cameraEntityOption.IsNone) { return Result.Err<bool, string>("Camera not found"); }

        ref readonly CameraComponent camera = ref world.Get<CameraComponent>(cameraEntityOption.Unwrap());
        return ExecutePlacement(world, camera.Camera, cardType, cost, mousePos, ref resources);
    }

    /// <summary>
    /// Executes the placement of a new wagon or updates an existing one based on the provided parameters.
    /// </summary>
    /// <param name="world">The ECS world instance containing all entities and components.</param>
    /// <param name="camera">The 3D camera used for raycasting to determine placement location.</param>
    /// <param name="type">The type of wagon to place or update.</param>
    /// <param name="cost">The resource cost for the placement action.</param>
    /// <param name="mousePosition">The mouse position in screen space used for raycasting.</param>
    /// <param name="resources">The resource component containing information on available resources.</param>
    /// <returns>
    /// A Result object containing a boolean indicating success or failure, or an error message describing the reason for failure.
    /// </returns>
    private Result<bool, string> ExecutePlacement(
        World world, Camera3D camera, WagonType type, int cost, Vector2 mousePosition, ref ResourceComponent resources
    ) {
        Option<(Node3D Collider, Vector3 Position)> rayResult =
            PerformRaycast(camera, mousePosition).Bind(GetColliderData);
        if (rayResult.IsNone) { return Result.Err<bool, string>("Nothing hit"); }

        (Node3D collider, Vector3 position) = rayResult.Unwrap();
        if (collider.HasMeta("EntityId")) {
            int entityId = (int)collider.GetMeta("EntityId");
            if (ApplyCardToWagon(world, new Entity(entityId), type)) {
                resources.Scrap -= cost;
                return Result.Ok<bool, string>(true);
            }

            return Result.Err<bool, string>("Cannot apply to this wagon");
        }

        if (!collider.HasMeta("IsFloor") || !IsValidFloorSpace(world, position)) {
            return Result.Err<bool, string>("Invalid placement area");
        }

        int maxSlot = GetMaxSlot(world);
        CreateNewWagon(world, maxSlot + 1, 0, type);
        resources.Scrap -= cost;
        return Result.Ok<bool, string>(true);
    }

    /// <summary>
    /// Extracts collider and position data from the provided result dictionary.
    /// </summary>
    /// <param name="result">The dictionary containing raycast hit data, expected to have "collider" and "position" keys.</param>
    /// <returns>An <see cref="Option{T}"/> containing a tuple with the collided <see cref="Node3D"/> and its hit position
    /// as a <see cref="Vector3"/>, or <see cref="Option{T}.None" />
    /// if the required data is missing.</returns>
    private static Option<(Node3D Collider, Vector3 Position)> GetColliderData(Dictionary result) {
        if (result.TryGetValue("collider", out Variant colliderVariant) &&
            result.TryGetValue("position", out Variant positionVariant)) {
            return Option<(Node3D, Vector3)>.Some(((Node3D)colliderVariant, (Vector3)positionVariant));
        }

        return Option<(Node3D, Vector3)>.None;
    }

    /// <summary>
    /// Processes a collision to determine the appropriate preview configuration for placing or upgrading a wagon.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="collider">The node corresponding to the object that was collided with.</param>
    /// <param name="hitPosition">The position of the collision in world coordinates.</param>
    /// <param name="cardType">The type of wagon card being processed.</param>
    /// <returns>An Option containing a <see cref="PreviewConfig"/> if a valid preview configuration can be generated; otherwise, None.</returns>
    private Option<PreviewConfig> ProcessCollision(
        World world, Node3D collider, Vector3 hitPosition, WagonType cardType
    ) {
        if (collider.HasMeta("EntityId")) {
            int entityId = (int)collider.GetMeta("EntityId");
            Entity hitEntity = new(entityId);
            if (!world.IsAlive(hitEntity)) { return Option<PreviewConfig>.None; }

            ref WagonSlotComponent hitSlot = ref world.Get<WagonSlotComponent>(hitEntity);
            (int highestLayer, WagonType topType) = FindTopWagon(world, hitSlot.SlotIndex);

            if (topType == WagonType.Locomotive) { return Option<PreviewConfig>.None; }

            bool isUpgrade = topType == cardType;
            return Option<PreviewConfig>.Some(
                new PreviewConfig {
                    Position =
                        TrainLayout.GetLocalPosition(hitSlot.SlotIndex, isUpgrade ? highestLayer : highestLayer + 1),
                    Scale = isUpgrade ? new Vector3(1.1f, 1.1f, 1.1f) : Vector3.One,
                    AlbedoColor = isUpgrade ? new Color(1, 0.8f, 0.2f, 0.6f) : new Color(0.2f, 0.6f, 1, 0.6f),
                    Emission = isUpgrade ? new Color(1, 0.8f, 0.2f) : new Color(0.2f, 0.6f, 1),
                    Visible = true
                }
            );
        }

        if (collider.HasMeta("IsFloor") && IsValidFloorSpace(world, hitPosition)) {
            return Option<PreviewConfig>.Some(
                new PreviewConfig {
                    Position = TrainLayout.GetLocalPosition(GetMaxSlot(world) + 1, 0),
                    Scale = Vector3.One,
                    AlbedoColor = new Color(0.2f, 1, 0.2f, 0.6f),
                    Emission = new Color(0.2f, 1, 0.2f),
                    Visible = true
                }
            );
        }

        return Option<PreviewConfig>.None;
    }

    /// <summary>
    /// Determines whether the given position is a valid location for placing a wagon
    /// on the train's floor space.
    /// </summary>
    /// <param name="world">The world context containing train-related entities and data.</param>
    /// <param name="globalHitPosition">The global hit position to be verified as floor space.</param>
    /// <returns>
    /// True if the position is a valid floor space for a wagon; otherwise, false.
    /// </returns>
    private bool IsValidFloorSpace(World world, Vector3 globalHitPosition) {
        // The train extends in the NEGATIVE X direction.
        // lastWagonX is a large negative value (e.g. -20 for slot 4).
        // A floor hit is valid if it lands near or behind that last wagon,
        // meaning its local X must be <= lastWagonX + half-a-wagon of tolerance.
        Vector3 localHit = _trainRoot.ToLocal(globalHitPosition);
        float lastWagonX = -GetMaxSlot(world) * (TrainLayout.WagonLength + TrainLayout.WagonGap);

        // Limit Z to stay near the train track width
        bool withinWidth = Mathf.Abs(localHit.Z) < TrainLayout.WagonWidth * 1.5f;
        // Allow clicking anywhere from the second-to-last wagon onwards to the back
        bool withinLength = localHit.X <= lastWagonX + TrainLayout.WagonLength;

        return withinWidth && withinLength;
    }

    /// <summary>
    /// Determines the maximum slot index currently occupied by wagons within the train system.
    /// </summary>
    /// <param name="world">The ECS world containing all entities and their components.</param>
    /// <returns>The highest slot index occupied by any wagon in the system.</returns>
    private static int GetMaxSlot(World world) {
        int max = 0;
        foreach (Entity entity in world.Query<WagonSlotComponent>()) {
            int slot = world.Get<WagonSlotComponent>(entity).SlotIndex;
            if (slot > max) { max = slot; }
        }

        return max;
    }

    /// <summary>
    /// Retrieves the highest layer and corresponding wagon type from a specified slot index within the train system.
    /// </summary>
    /// <param name="world">The ECS world containing all entities and components.</param>
    /// <param name="slotIndex">The index of the wagon slot to search for the top wagon.</param>
    /// <returns>A tuple containing the highest layer index and the wagon type found at that layer. If no wagons are found, returns -1 for the layer and the default wagon type.</returns>
    private static (int layer, WagonType type) FindTopWagon(World world, int slotIndex) {
        int topLayer = -1;
        WagonType topType = WagonType.Locomotive;
        bool found = false;

        world.ForEach((ref WagonSlotComponent slot, ref WagonTypeComponent wagonType) => {
            if (slot.SlotIndex != slotIndex) { return; }

            if (slot.Layer <= topLayer) { return; }

            topLayer = slot.Layer;
            topType = wagonType.Type;
            found = true;
        }
        );

        return found ? (topLayer, topType) : (-1, WagonType.Locomotive);
    }

    /// <summary>
    /// Applies a card to a specific wagon, either augmenting an existing wagon's stats or creating a new wagon,
    /// based on the card type and the wagon's current top layer.
    /// </summary>
    /// <param name="world">The game world instance, providing access to entities and components.</param>
    /// <param name="hitEntity">The entity representing the wagon slot where the card is to be applied.</param>
    /// <param name="cardType">The type of the wagon card being applied.</param>
    /// <returns>True if the card was successfully applied, false otherwise.</returns>
    private static bool ApplyCardToWagon(World world, Entity hitEntity, WagonType cardType) {
        if (!world.IsAlive(hitEntity)) { return false; }

        ref WagonSlotComponent hitSlot = ref world.Get<WagonSlotComponent>(hitEntity);
        Entity topEntity = hitEntity;
        int highestLayer = -1;

        foreach (Entity entity in world.Query<WagonSlotComponent, WagonTypeComponent>()) {
            ref WagonSlotComponent slot = ref world.Get<WagonSlotComponent>(entity);
            if (slot.SlotIndex != hitSlot.SlotIndex || slot.Layer <= highestLayer) { continue; }

            highestLayer = slot.Layer;
            topEntity = entity;
        }

        WagonType topType = world.Get<WagonTypeComponent>(topEntity).Type;
        if (topType == WagonType.Locomotive) { return false; }

        if (topType == cardType) {
            ref HealthComponent health = ref world.Get<HealthComponent>(topEntity);
            health.Max += 50f;
            health.Current += 50f;

            if (cardType == WagonType.Combat && world.Has<TurretComponent>(topEntity)) {
                ref TurretComponent turret = ref world.Get<TurretComponent>(topEntity);
                turret.Damage += 10f;
                turret.FireRate += 3f;
            }

            return true;
        }

        CreateNewWagon(world, hitSlot.SlotIndex, highestLayer + 1, cardType);
        return true;
    }

    /// <summary>
    /// Creates a new wagon entity in the specified slot and layer, assigning components
    /// such as type, health, and appearance based on the provided wagon type.
    /// </summary>
    /// <param name="world">The simulation world where the new wagon entity will be added.</param>
    /// <param name="slot">The slot index where the wagon will be placed.</param>
    /// <param name="layer">The layer index to assign to the wagon within the specified slot.</param>
    /// <param name="type">The type of the wagon determining its attributes, appearance, and functionality.</param>
    private static void CreateNewWagon(World world, int slot, int layer, WagonType type) {
        Color tint = type switch {
            WagonType.Combat => TrainLayout.ColorCombat,
            WagonType.Living => TrainLayout.ColorLiving,
            WagonType.Storage => TrainLayout.ColorStorage,
            WagonType.Research => TrainLayout.ColorResearch,
            _ => Colors.Gray
        };

        Entity entity = world.CreateEntity();
        world.Add(entity, new WagonTypeComponent { Type = type, BlueprintId = "card_spawn" });
        world.Add(entity, new WagonSlotComponent { SlotIndex = slot, Layer = layer });
        world.Add(entity, new HealthComponent { Max = 150f, Current = 150f });
        world.Add(entity, new RenderableComponent { Tint = tint, Label = type.ToString().ToUpper() });

        if (type == WagonType.Combat) {
            world.Add(entity, new TurretComponent { Range = 35f, Damage = 15f, FireRate = 6f });
        }
    }
}
