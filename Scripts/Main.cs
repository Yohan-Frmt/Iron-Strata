using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Components.Camera;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Autoloads;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Map;
using IronStrata.Scripts.Registry;
using IronStrata.Scripts.Systems.Camera;
using IronStrata.Scripts.Systems.Combat;
using IronStrata.Scripts.Systems.Debug;
using IronStrata.Scripts.Systems.Map;
using IronStrata.Scripts.Systems.Render;
using IronStrata.Scripts.Systems.Shared;
using IronStrata.Scripts.Systems.Train;
using IronStrata.Scripts.UI;
using WorldEnvironment = IronStrata.Scenes.WorldEnvironment;

namespace IronStrata.Scripts;

/// <summary>
/// Serves as the core manager of the game scene, handling essential gameplay mechanics and scene interactions.
/// </summary>
/// <remarks>
/// This class acts as the central orchestrator for player input, card system functionality, and visual previews.
/// It integrates various game systems, such as the construction logic and camera interactions, ensuring smooth
/// and intuitive gameplay mechanics. Extends Node3D to leverage 3D scene management capabilities in Godot.
/// </remarks>
public partial class Main : Node3D {
    /// <summary>
    /// Represents the UI element used to manage and display the player's hand within the game interface.
    /// This container facilitates dynamic interaction and organization of hand-related UI components.
    /// </summary>
    private Control _handContainer;

    /// <summary>
    /// References the preloaded card scene used within the application.
    /// This variable facilitates the dynamic instancing of card objects during runtime.
    /// </summary>
    private PackedScene _cardScene;

    /// <summary>
    /// Represents the simulation world within the application's ECS (Entity Component System) framework.
    /// This variable serves as the core context for managing entities, components, and systems.
    /// </summary>
    private World _world;

    /// <summary>
    /// Manages the construction-related processes within the application's system.
    /// This variable is responsible for coordinating subsystem interactions
    /// to handle the initiation, management, and completion of construction tasks.
    /// </summary>
    private ConstructionSystem _constructionSystem;

    /// <summary>
    /// Manages the tactical pause functionality within the game.
    /// This variable is responsible for coordinating the pause state, including
    /// interaction with UI components and ensuring seamless system performance
    /// when the game is paused or resumed.
    /// </summary>
    private TacticalPauseSystem _pauseSystem;

    /// <summary>
    /// Manages the camera operations within the 3D scene.
    /// Responsible for handling camera control, integration, and overall
    /// behavior as part of the system framework.
    /// </summary>
    private CameraSystem _cameraSystem;

    /// <summary>
    /// Represents the root node of the 3D world in the scene.
    /// This variable is intended to reference the parent node
    /// under which all other world elements and objects are organized.
    /// </summary>
    [Export] private Node3D _worldRoot;

    /// <summary>
    /// Executes once the node has entered the scene tree. Used to perform any setup
    /// or initialization specific to this node, such as configuring properties,
    /// establishing connections, or preparing runtime behavior.
    /// </summary>
    public override void _Ready() {
        _world = GameWorld.Instance.World;
        WorldEnvironment.Setup(this);

        Node3D trainRoot = new() { Name = "TrainRoot" };
        Node3D enemyRoot = new() { Name = "EnemyRoot" };
        AddChild(trainRoot);
        AddChild(enemyRoot);

        SpotLight3D headlight = new() {
            Position = new Vector3(0f, 3f, 0f),
            Rotation = new Vector3(0, Mathf.DegToRad(-90), 0),
            SpotRange = 250f,
            SpotAngle = 40f,
            LightEnergy = 15f,
            LightColor = new Color(1f, 0.9f, 0.7f),
            ShadowEnabled = true,
            LightVolumetricFogEnergy = 4f
        };
        trainRoot.AddChild(headlight);

        SpringArm3D springArm = new() {
            SpringLength = 35f,
            ProcessMode = ProcessModeEnum.Always,
            Rotation = new Vector3(Mathf.DegToRad(-45f), 0f, 0f),
            CollisionMask = 2
        };

        Camera3D camera = new() { ProcessMode = ProcessModeEnum.Always };
        trainRoot.AddChild(springArm);
        springArm.AddChild(camera);
        camera.MakeCurrent();
        _cameraSystem = new CameraSystem();

        Entity cameraEntity = _world.CreateEntity();
        _world.Add(
            cameraEntity,
            new CameraComponent { SpringArm = springArm, Camera = camera, TargetRotation = springArm.Rotation }
        );

        MeshInstance3D floor = new() { Name = "Floor", Mesh = new PlaneMesh { Size = new Vector2(10000f, 10000f) } };
        floor.SetSurfaceOverrideMaterial(0, new StandardMaterial3D { AlbedoColor = new Color(0.15f, 0.15f, 0.18f) });

        StaticBody3D floorBody = new() { Name = "FloorBody" };
        CollisionShape3D floorShape = new() {
            Shape = new BoxShape3D { Size = new Vector3(2000f, 1f, 2000f) },
            Position = new Vector3(0, -0.5f, 0)
        };
        floorBody.AddChild(floorShape);
        floor.AddChild(floorBody);
        AddChild(floor);

        CanvasLayer hud = new() { Name = "HUD", Layer = 1 };
        AddChild(hud);

        Minimap minimap = new();
        minimap.SetPosition(new Vector2(GetViewport().GetVisibleRect().Size.X - 300, 20));
        hud.AddChild(minimap);

        MapOverlay mapOverlay = new();
        hud.AddChild(mapOverlay);

        ColorRect pauseOverlay = new() {
            Color = new Color(0, 0, 0, 0.3f),
            AnchorsPreset = (int)Control.LayoutPreset.FullRect,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        Entity stateEntity = _world.CreateEntity();
        _world.Add(stateEntity, new GameStateComponent());

        Label pauseLabel = new() {
            Text = "PAUSE TACTIQUE",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AnchorsPreset = (int)Control.LayoutPreset.Center
        };

        Button pauseButton =
            GetNode<Button>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxAction/PauseButton");
        if (pauseButton != null) {
            pauseButton.Pressed += () => _pauseSystem.TriggerPause();
            pauseButton.ProcessMode = ProcessModeEnum.Always;
        }
        else { GD.PrintErr("Le bouton de pause est introuvable, vérifiez l'Access as Unique Name !"); }

        _pauseSystem = new TacticalPauseSystem(pauseOverlay);
        pauseOverlay.AddChild(pauseLabel);
        hud.AddChild(pauseOverlay);
        pauseOverlay.ProcessMode = ProcessModeEnum.Always;

        Label speedLabel = new() { Position = new Vector2(24f, 20f) };
        speedLabel.AddThemeColorOverride("font_color", new Color(0.55f, 0.70f, 1.0f));
        hud.AddChild(speedLabel);

        Label scrapLabel = GetNode<Label>("UI/VBoxMainLayout/PanelTopBar/Margin/HBox/HBoxLeftStats/ScrapLabel");
        Button drawButton =
            GetNode<Button>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxAction/DrawButton");
        _handContainer =
            GetNode<Control>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxHand/HBoxHandContainer");
        Control bottomHud = GetNode<Control>("UI/VBoxMainLayout/PanelBottomBar");

        _cardScene = GD.Load<PackedScene>("res://Scenes/Cards/card_ui.tscn");
        drawButton.Pressed += DrawCard;

        Entity trainEntity = _world.CreateEntity();
        _world.Add(trainEntity, new TrainMovementComponent { MaxSpeed = 5f, Acceleration = 1.0f, Deceleration = 5.0f });

        Entity mapEntity = _world.CreateEntity();
        MapComponent mapComponent = new();
        List<List<MapNode>> mapData = new MapGenerator().GenerateMap();
        foreach (List<MapNode> layer in mapData) {
            List<int> layerIds = new(layer.Count);
            foreach (MapNode node in layer) { layerIds.Add(node.Id); }

            mapComponent.Layers.Add(layerIds);
            foreach (MapNode node in layer) { mapComponent.AllNodes[node.Id] = node; }
        }

        _world.Add(mapEntity, mapComponent);

        int startNodeId = mapComponent.Layers[0][0];
        int targetNodeId = mapComponent.AllNodes[startNodeId].NextNodes[0];

        _world.Add(
            mapEntity,
            new LocationComponent {
                CurrentNodeId = startNodeId,
                TargetNodeId = targetNodeId,
                IsInTransit = true,
                TravelProgress = 0f
            }
        );

        _world.Add(mapEntity, new ResourceComponent { Scrap = ResourceRegistry.StartingScrap });

        MeshInstance3D previewGhost = new() {
            Name = "PreviewGhost",
            Visible = false,
            Mesh = new BoxMesh {
                Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth)
            }
        };
        previewGhost.SetSurfaceOverrideMaterial(
            0,
            new StandardMaterial3D {
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                EmissionEnabled = true,
                EmissionEnergyMultiplier = 0.5f
            }
        );
        trainRoot.AddChild(previewGhost);

        _constructionSystem = new ConstructionSystem(previewGhost, bottomHud, trainRoot);

        Godot.WorldEnvironment environmentNode = GetNode<Godot.WorldEnvironment>("WorldEnvironment");

        GameWorld.Instance.Runner
            .Add(new TrainMovementSystem(speedLabel))
            .Add(new RailLightSystem(_worldRoot))
            .Add(new FogSystem(environmentNode))
            .Add(_cameraSystem)
            .Add(_pauseSystem)
            .Add(new InputSystem())
            .Add(new WagonConnectionSystem())
            .Add(new MapSystem(trainRoot))
            .Add(new MapRenderSystem(floor))
            .Add(new EnemySystem(trainRoot))
            .Add(new TurretSystem(trainRoot))
            .Add(new ResourceSystem(scrapLabel, drawButton))
            .Add(_constructionSystem)
            .Add(new RenderSystem(trainRoot, _world))
            .Add(new DebugRenderSystem(trainRoot))
            .Add(new WagonHealthUiSystem())
            .Add(new EnemyMultiMeshSystem(enemyRoot));

        SpawnTestTrain(_world);
    }

    /// <summary>
    /// Processes input events dispatched to the node during the game loop.
    /// Used to delegate input handling to relevant systems or perform game-specific logic.
    /// </summary>
    /// <param name="event">The input event to be processed, such as key presses, mouse motion, or other user interactions.</param>
    public override void _Input(InputEvent @event) => _cameraSystem?.OnInput(@event);

    /// <summary>
    /// Draws a card from the deck if the player meets the resource requirements
    /// and the maximum hand size has not been exceeded. Deducts the required
    /// resource cost for drawing a card, creates a new card of a random type
    /// (Combat or Storage), and adds it to the player's hand container.
    /// </summary>
    private void DrawCard() {
        Option<Entity> resourceEntityOption = _world.QueryFirst<ResourceComponent>();
        if (resourceEntityOption.IsNone) { return; }

        ref ResourceComponent resources = ref _world.Get<ResourceComponent>(resourceEntityOption.Unwrap());
        if (resources.Scrap < ResourceRegistry.CardDrawCost || _handContainer.GetChildCount() >= 5) { return; }

        resources.Scrap -= ResourceRegistry.CardDrawCost;
        CardUi newCard = _cardScene.Instantiate<CardUi>();
        WagonType randomType = GD.Randf() > 0.5f ? WagonType.Combat : WagonType.Storage;
        _handContainer.AddChild(newCard);
        newCard.Setup(randomType);
    }

    /// <summary>
    /// Attempts to play a wagon card by constructing the specified wagon type at a given position,
    /// provided that all conditions, such as resources and placement validity, are satisfied.
    /// </summary>
    /// <param name="cardType">The type of wagon card to be played.</param>
    /// <param name="cost">The resource cost required to play the card.</param>
    /// <param name="mousePosition">The global mouse position where the wagon is to be constructed.</param>
    /// <returns>
    /// A result object containing a boolean indicating whether the card was successfully played,
    /// and an optional string providing additional error information if the attempt fails.
    /// </returns>
    public Result<bool, string> TryPlayCard(WagonType cardType, int cost, Vector2 mousePosition) =>
        _constructionSystem.TryPlayCard(_world, cardType, cost, mousePosition);

    /// <summary>
    /// Updates the 3D preview of a wagon currently being placed during the construction phase.
    /// </summary>
    /// <param name="cardType">The type of the wagon, representing its functionality or category, to be displayed in the preview.</param>
    /// <param name="mousePosition">The current mouse position in screen coordinates, used to determine the location of the preview in the game world.</param>
    public void UpdatePreview(WagonType cardType, Vector2 mousePosition) =>
        _constructionSystem.UpdatePreview(_world, cardType, mousePosition);

    /// <summary>
    /// Hides the construction preview in the game, removing any visual or interactive elements
    /// associated with the player's current construction action. This method ensures that the
    /// preview state is cleared and no longer displayed to the user, effectively resetting the
    /// construction preview system.
    /// </summary>
    public void HidePreview() => _constructionSystem.HidePreview();

    /// <summary>
    /// Spawns a test train within the specified world. Creates a sequence of wagons including a locomotive,
    /// combat wagons, and living wagons. The wagons are linked together to form a train with specific properties
    /// for each entity.
    /// </summary>
    /// <param name="world">The world instance in which the test train entities will be created and registered,
    /// and to which the wagons are connected.</param>
    private static void SpawnTestTrain(World world) {
        Entity locomotive = CreateWagon(world, 0, 0, WagonType.Locomotive, "loco", 500f, TrainLayout.ColorLoco, "LOCO");

        Entity combat = CreateWagon(
            world, 1, 0, WagonType.Combat, "combat", 2000000f, TrainLayout.ColorCombat, "COMBAT"
        );
        world.Add(combat, new ConnectionComponent { PreviousEntityId = locomotive.Id, NextEntityId = -1, Integrity = 1f });

        Entity living = CreateWagon(
            world, 2, 0, WagonType.Living, "living", 2000000f, TrainLayout.ColorLiving, "LIVING"
        );
        world.Add(living, new ConnectionComponent { PreviousEntityId = combat.Id, NextEntityId = -1, Integrity = 1f });

        Entity combat2 = CreateWagon(
            world, 3, 0, WagonType.Combat, "combat", 2000000f, TrainLayout.ColorCombat, "COMBAT"
        );
        world.Add(combat2, new ConnectionComponent { PreviousEntityId = living.Id, NextEntityId = -1, Integrity = 1f });
    }

    /// <summary>
    /// Creates and initializes a new wagon entity in the specified world with the provided attributes,
    /// configuring its behavior, appearance, and additional components required for gameplay functionality.
    /// </summary>
    /// <param name="world">The world instance in which the wagon entity will be created and managed.</param>
    /// <param name="slot">The slot position of the wagon within the designated layer.</param>
    /// <param name="layer">The layer to which the wagon belongs, affecting its grouping and hierarchy.</param>
    /// <param name="type">The type of wagon, determining its role, functionality, and behavior (e.g., locomotive, combat).</param>
    /// <param name="blueprint">The blueprint identifier used to define the wagon's structure, appearance, and attributes.</param>
    /// <param name="health">The initial and maximum health value of the wagon, representing its durability.</param>
    /// <param name="tint">The color tint applied to the wagon's visual appearance.</param>
    /// <param name="label">The name or label assigned to the wagon for identification purposes.</param>
    /// <returns>The newly created wagon entity with all specified components initialized.</returns>
    private static Entity CreateWagon(
        World world, int slot, int layer, WagonType type, string blueprint, float health,
        Color tint, string label
    ) {
        Entity entity = world.CreateEntity();
        world.Add(entity, new WagonTypeComponent { Type = type, BlueprintId = blueprint });
        world.Add(entity, new WagonSlotComponent { SlotIndex = slot, Layer = layer });
        world.Add(entity, new HealthComponent { Max = health, Current = health });
        world.Add(entity, new RenderableComponent { Tint = tint, Label = label });

        if (type == WagonType.Combat) {
            world.Add(entity, new TurretComponent { Range = 30f, Damage = 15f, FireRate = 10f });
        }

        return entity;
    }
}
