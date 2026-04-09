using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Autoloads;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Map;
using IronStrata.Scripts.Registry;
using IronStrata.Scripts.Systems.Combat;
using IronStrata.Scripts.Systems.Debug;
using IronStrata.Scripts.Systems.Map;
using IronStrata.Scripts.Systems.Render;
using IronStrata.Scripts.Systems.Shared;
using IronStrata.Scripts.Systems.Train;
using IronStrata.Scripts.UI;

namespace IronStrata.Scripts;

/// <summary>
/// The main entry point and controller for the game scene.
/// It bootstraps the ECS world, registers systems, and manages the primary game loop setup.
/// </summary>
public partial class Main : Node
{
    private const float CameraLeadX = 8f;

    private Control _handContainer;
    private PackedScene _cardScene;

    private World _world;
    private ConstructionSystem _constructionSystem;
    private TacticalPauseSystem _pauseSystem;

    /// <summary>
    /// Sets up the initial game state, world, and ECS systems.
    /// </summary>
    public override void _Ready()
    {
        _world = GameWorld.Instance.World;
        SetupDebugLighting();

        var trainRoot = new Node3D { Name = "TrainRoot" };
        AddChild(trainRoot);
        var enemyRoot = new Node3D { Name = "EnemyRoot" };
        AddChild(enemyRoot);

        var camera = new Camera3D { 
            Name = "Camera", 
            Projection = Camera3D.ProjectionType.Orthogonal, 
            Size = 35f, 
            Position = new Vector3(CameraLeadX - 20f, 25f, 25f) 
        };
        AddChild(camera);
        camera.LookAt(new Vector3(CameraLeadX, 0f, 0f), Vector3.Up);
        camera.MakeCurrent();
        camera.ProcessMode = ProcessModeEnum.Always; 

        var floor = new MeshInstance3D { Name = "Floor" };
        floor.Mesh = new PlaneMesh { Size = new Vector2(2000f, 2000f) };
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

        var pauseOverlay = new ColorRect {
            Color = new Color(0, 0, 0, 0.3f), // Noir transparent
            AnchorsPreset = (int)Control.LayoutPreset.FullRect,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        
        var stateEntity = _world.CreateEntity();
        _world.Add(stateEntity, new GameStateComponent());
        
        var pauseLabel = new Label {
            Text = "PAUSE TACTIQUE",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AnchorsPreset = (int)Control.LayoutPreset.Center
        };
        
        _pauseSystem = new TacticalPauseSystem(pauseOverlay);
        var pauseButton = GetNode<Button>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxAction/PauseButton");
        if (pauseButton != null) {
            pauseButton.Pressed += () => _pauseSystem.TriggerPause();
            pauseButton.ProcessMode = ProcessModeEnum.Always;
        } else {
            GD.PrintErr("Le bouton de pause est introuvable, vérifiez l'Access as Unique Name !");
        }
        pauseOverlay.AddChild(pauseLabel);
        hud.AddChild(pauseOverlay);
        pauseOverlay.ProcessMode = ProcessModeEnum.Always;
        
        var speedLabel = new Label { Position = new Vector2(24f, 20f) };
        speedLabel.AddThemeColorOverride("font_color", new Color(0.55f, 0.70f, 1.0f));
        hud.AddChild(speedLabel);

        var scrapLabel = GetNode<Label>("UI/VBoxMainLayout/PanelTopBar/Margin/HBox/HBoxLeftStats/ScrapLabel");
        var drawButton = GetNode<Button>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxAction/DrawButton");
        _handContainer = GetNode<Control>("UI/VBoxMainLayout/PanelBottomBar/Margin/HBoxHUDColumns/VBoxHand/HBoxHandContainer");
        var bottomHud = GetNode<Control>("UI/VBoxMainLayout/PanelBottomBar");

        _cardScene = GD.Load<PackedScene>("res://Scenes/Cards/card_ui.tscn");
        drawButton.Pressed += DrawCard;

        // Initialize Train Entity.
        var trainEntity = _world.CreateEntity();
        _world.Add(trainEntity, new TrainMovementComponent { MaxSpeed = 5f, Acceleration = 1.0f, Deceleration = 5.0f });

        // Generate and initialize Map Entity.
        var mapEntity = _world.CreateEntity();
        var mapComp = new MapComponent();
        var mapData = new MapGenerator().GenerateMap();
        foreach (var layer in mapData)
        {
            mapComp.Layers.Add(layer.Select(n => n.Id).ToList());
            foreach (var node in layer) mapComp.AllNodes[node.Id] = node;
        }
        _world.Add(mapEntity, mapComp);

        // Set starting position on the map.
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

        // Construction preview setup.
        var previewGhost = new MeshInstance3D { Name = "PreviewGhost", Visible = false };
        previewGhost.Mesh = new BoxMesh { 
            Size = new Vector3(TrainLayout.WagonLength, TrainLayout.WagonHeight, TrainLayout.WagonWidth) 
        };
        previewGhost.SetSurfaceOverrideMaterial(0, new StandardMaterial3D { 
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha, 
            EmissionEnabled = true, 
            EmissionEnergyMultiplier = 0.5f 
        });
        trainRoot.AddChild(previewGhost);

        _constructionSystem = new ConstructionSystem(camera, previewGhost, bottomHud, trainRoot);

        // Register all logic systems to the runner.
        GameWorld.Instance.Runner
            .Add(new TrainMovementSystem(trainRoot, camera, speedLabel))
            .Add(_pauseSystem)
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

        // Initial wagons for testing.
        SpawnTestTrain(_world);
    }

    /// <summary>
    /// Event handler for the draw card button.
    /// </summary>
    private void DrawCard()
    {
        var resEntity = _world.Query<ResourceComponent>().FirstOrDefault();
        if (resEntity == null) return;
        
        var resources = _world.Get<ResourceComponent>(resEntity);
        if (resources.Scrap < ResourceRegistry.CardDrawCost || _handContainer.GetChildCount() >= 5) return;
        
        resources.Scrap -= ResourceRegistry.CardDrawCost;
        var newCard = _cardScene.Instantiate<Scripts.UI.CardUi>();
        var randomType = GD.Randf() > 0.5f ? WagonType.Combat : WagonType.Storage;
        _handContainer.AddChild(newCard);
        newCard.Setup(randomType);
    }

    // Bridge methods between UI and Construction System.
    public bool TryPlayCard(WagonType cardType, int cost, Vector2 mousePos) => _constructionSystem.TryPlayCard(_world, cardType, cost, mousePos);
    public void UpdatePreview(WagonType cardType, Vector2 mousePos) => _constructionSystem.UpdatePreview(_world, cardType, mousePos);
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
    private static Entity CreateWagon(World world, int slot, int layer, WagonType type, string blueprint, float health, Color tint, string label)
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

    /// <summary>
    /// Sets up basic lighting and environment for the 3D scene.
    /// </summary>
    private void SetupDebugLighting()
    {
        AddChild(new DirectionalLight3D { 
            LightEnergy = 1.5f, 
            LightColor = Colors.White, 
            Rotation = new Vector3(Mathf.DegToRad(-50f), Mathf.DegToRad(-30f), 0f) 
        });
        
        var env = new Environment { 
            BackgroundMode = Environment.BGMode.Color, 
            BackgroundColor = new Color(0.1f, 0.1f, 0.12f) 
        };
        AddChild(new Godot.WorldEnvironment { Environment = env });
    }
}