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
/// The main entry point and controller for the game scene.
/// It bootstraps the ECS world, registers systems, and manages the primary game loop setup.
/// </summary>
public partial class Main : Node3D
{
    private Control _handContainer;
    private PackedScene _cardScene;

    private World _world;
    private ConstructionSystem _constructionSystem;
    private TacticalPauseSystem _pauseSystem;
    private CameraSystem _cameraSystem;
    [Export] private Node3D _worldRoot;

    /// <summary>
    /// Sets up the initial game state, world, and ECS systems.
    /// </summary>
    public override void _Ready()
    {
        _world = GameWorld.Instance.World;
        WorldEnvironment.Setup(this);

        var trainRoot = new Node3D { Name = "TrainRoot" };
        var enemyRoot = new Node3D { Name = "EnemyRoot" };
        AddChild(trainRoot);
        AddChild(enemyRoot);

        var headlight = new SpotLight3D
        {
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

        var springArm = new SpringArm3D
        {
            SpringLength = 35f,
            ProcessMode = ProcessModeEnum.Always,
            Rotation = new Vector3(Mathf.DegToRad(-45f), 0f, 0f),
            CollisionMask = 2
        };

        var camera = new Camera3D
        {
            ProcessMode = ProcessModeEnum.Always
        };
        trainRoot.AddChild(springArm);
        springArm.AddChild(camera);
        camera.MakeCurrent();
        _cameraSystem = new CameraSystem();

        var camEntity = _world.CreateEntity();
        _world.Add(camEntity, new CameraComponent
        {
            SpringArm = springArm,
            Camera = camera,
            TargetRotation = springArm.Rotation
        });

        var floor = new MeshInstance3D { Name = "Floor" };
        floor.Mesh = new PlaneMesh { Size = new Vector2(10000f, 10000f) };
        floor.SetSurfaceOverrideMaterial(0, new StandardMaterial3D { AlbedoColor = new Color(0.15f, 0.15f, 0.18f) });

        var floorBody = new StaticBody3D { Name = "FloorBody" };
        var floorShape = new CollisionShape3D { Shape = new BoxShape3D { Size = new Vector3(2000f, 1f, 2000f) } };
        floorShape.Position = new Vector3(0, -0.5f, 0);
        floorBody.AddChild(floorShape);
        floor.AddChild(floorBody);
        AddChild(floor);

        var hud = new CanvasLayer { Name = "HUD", Layer = 1 };
        AddChild(hud);

        var minimap = new Minimap();
        minimap.SetPosition(new Vector2(GetViewport().GetVisibleRect().Size.X - 300, 20));
        hud.AddChild(minimap);

        var pauseOverlay = new ColorRect
        {
            Color = new Color(0, 0, 0, 0.3f),
            AnchorsPreset = (int)Control.LayoutPreset.FullRect,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        var stateEntity = _world.CreateEntity();
        _world.Add(stateEntity, new GameStateComponent());

        var pauseLabel = new Label
        {
            Text = "PAUSE TACTIQUE",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AnchorsPreset = (int)Control.LayoutPreset.Center
        };

        var pauseButton =
            GetNode<Button>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxAction/PauseButton");
        if (pauseButton != null)
        {
            pauseButton.Pressed += () => _pauseSystem.TriggerPause();
            pauseButton.ProcessMode = ProcessModeEnum.Always;
        }
        else
        {
            GD.PrintErr("Le bouton de pause est introuvable, vérifiez l'Access as Unique Name !");
        }

        _pauseSystem = new TacticalPauseSystem(pauseOverlay);
        pauseOverlay.AddChild(pauseLabel);
        hud.AddChild(pauseOverlay);
        pauseOverlay.ProcessMode = ProcessModeEnum.Always;

        var speedLabel = new Label { Position = new Vector2(24f, 20f) };
        speedLabel.AddThemeColorOverride("font_color", new Color(0.55f, 0.70f, 1.0f));
        hud.AddChild(speedLabel);

        var scrapLabel = GetNode<Label>("UI/VBoxMainLayout/PanelTopBar/Margin/HBox/HBoxLeftStats/ScrapLabel");
        var drawButton =
            GetNode<Button>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxAction/DrawButton");
        _handContainer =
            GetNode<Control>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxHand/HBoxHandContainer");
        var bottomHud = GetNode<Control>("UI/VBoxMainLayout/PanelBottomBar");

        _cardScene = GD.Load<PackedScene>("res://Scenes/Cards/card_ui.tscn");
        drawButton.Pressed += DrawCard;

        var trainEntity = _world.CreateEntity();
        _world.Add(trainEntity, new TrainMovementComponent { MaxSpeed = 5f, Acceleration = 1.0f, Deceleration = 5.0f });

        var mapEntity = _world.CreateEntity();
        var mapComp = new MapComponent();
        var mapData = new MapGenerator().GenerateMap();
        foreach (var layer in mapData)
        {
            var layerIds = new List<int>(layer.Count);
            foreach (var node in layer) layerIds.Add(node.Id);
            mapComp.Layers.Add(layerIds);
            foreach (var node in layer) mapComp.AllNodes[node.Id] = node;
        }

        _world.Add(mapEntity, mapComp);

        var startNodeId = mapComp.Layers[0][0];
        var targetNodeId = mapComp.AllNodes[startNodeId].NextNodes[0];

        _world.Add(mapEntity, new LocationComponent
        {
            CurrentNodeId = startNodeId,
            TargetNodeId = targetNodeId,
            IsInTransit = true,
            TravelProgress = 0f
        });

        _world.Add(mapEntity, new ResourceComponent { Scrap = ResourceRegistry.StartingScrap });

        var previewGhost = new MeshInstance3D { Name = "PreviewGhost", Visible = false };
        previewGhost.Mesh = new BoxMesh
        {
            Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth)
        };
        previewGhost.SetSurfaceOverrideMaterial(0, new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            EmissionEnabled = true,
            EmissionEnergyMultiplier = 0.5f
        });
        trainRoot.AddChild(previewGhost);

        _constructionSystem = new ConstructionSystem(previewGhost, bottomHud, trainRoot);

        var envNode = GetNode<Godot.WorldEnvironment>("WorldEnvironment");

        GameWorld.Instance.Runner
            .Add(new TrainMovementSystem(speedLabel))
            .Add(new RailLightSystem(_worldRoot))
            .Add(new FogSystem(envNode))
            .Add(_cameraSystem)
            .Add(_pauseSystem)
            .Add(new MapInputSystem())
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

    public override void _Input(InputEvent @event)
    {
        _cameraSystem?.OnInput(@event);
    }

    /// <summary>
    /// Draws a card from the deck if the player has sufficient resources
    /// and the hand container has not reached its maximum capacity.
    /// Deducts the card draw cost from the player's resources
    /// and generates a new card of a random type (Combat or Storage)
    /// which is added to the player's hand.
    /// </summary>
    private void DrawCard()
    {
        var resEntityOpt = _world.QueryFirst<ResourceComponent>();
        if (resEntityOpt.IsSome)
        {
            ref var resources = ref _world.Get<ResourceComponent>(resEntityOpt.Unwrap());
            if (resources.Scrap < ResourceRegistry.CardDrawCost || _handContainer.GetChildCount() >= 5) return;
            resources.Scrap -= ResourceRegistry.CardDrawCost;
            var newCard = _cardScene.Instantiate<CardUi>();
            var randomType = GD.Randf() > 0.5f ? WagonType.Combat : WagonType.Storage;
            _handContainer.AddChild(newCard);
            newCard.Setup(randomType);
        }
    }

    /// <summary>
    /// Attempts to play a card by building the specified wagon type at the mouse position, if the conditions are met.
    /// </summary>
    /// <param name="cardType">The type of wagon card to play.</param>
    /// <param name="cost">The resource cost to play the card.</param>
    /// <param name="mousePos">The global position of the mouse where the card is to be played.</param>
    /// <returns>Returns true if the card was successfully played; otherwise, false.</returns>
    public Result<bool, string> TryPlayCard(WagonType cardType, int cost, Vector2 mousePos) =>
        _constructionSystem.TryPlayCard(_world, cardType, cost, mousePos);

    /// <summary>
    /// Updates the 3D preview of a wagon being placed during construction.
    /// </summary>
    /// <param name="cardType">The type of wagon being previewed.</param>
    /// <param name="mousePos">The current position of the mouse, used to determine the preview's location in the game world.</param>
    public void UpdatePreview(WagonType cardType, Vector2 mousePos) =>
        _constructionSystem.UpdatePreview(_world, cardType, mousePos);

    /// <summary>
    /// Hides the wagon placement preview ghost from the game view.
    /// This method ensures the visual representation of a card preview is no longer displayed.
    /// </summary>
    public void HidePreview() => _constructionSystem.HidePreview();

    /// <summary>
    /// Spawns a basic locomotive and a few starting wagons.
    /// </summary>
    private static void SpawnTestTrain(World world)
    {
        var loco = CreateWagon(world, 0, 0, WagonType.Locomotive, "loco", 500f, TrainLayout.ColorLoco, "LOCO");

        var combat = CreateWagon(world, 1, 0, WagonType.Combat, "combat", 2000000f, TrainLayout.ColorCombat, "COMBAT");
        world.Add(combat, new ConnectionComponent { PreviousEntityId = loco.Id, NextEntityId = -1, Integrity = 1f });

        var living = CreateWagon(world, 2, 0, WagonType.Living, "living", 2000000f, TrainLayout.ColorLiving, "LIVING");
        world.Add(living, new ConnectionComponent { PreviousEntityId = combat.Id, NextEntityId = -1, Integrity = 1f });

        var combat2 = CreateWagon(world, 3, 0, WagonType.Combat, "combat", 2000000f, TrainLayout.ColorCombat, "COMBAT");
        world.Add(combat2, new ConnectionComponent { PreviousEntityId = living.Id, NextEntityId = -1, Integrity = 1f });
    }

    /// <summary>
    /// Factory method to create a wagon entity with standard components.
    /// </summary>
    private static Entity CreateWagon(World world, int slot, int layer, WagonType type, string blueprint, float health,
        Color tint, string label)
    {
        var entity = world.CreateEntity();
        world.Add(entity, new WagonTypeComponent { Type = type, BlueprintId = blueprint });
        world.Add(entity, new WagonSlotComponent { SlotIndex = slot, Layer = layer });
        world.Add(entity, new HealthComponent { Max = health, Current = health });
        world.Add(entity, new RenderableComponent { Tint = tint, Label = label });

        if (type == WagonType.Combat)
            world.Add(entity, new TurretComponent { Range = 30f, Damage = 15f, FireRate = 10f });

        return entity;
    }
}