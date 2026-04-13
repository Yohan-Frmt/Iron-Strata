using System;
using System.Linq;
using Godot;
using IronStrata.Scenes;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.UI;

/// <summary>
/// Controls the behavior and visual representation of a wagon card in the player's hand.
/// Handles drag-and-drop interactions for construction.
/// </summary>
public partial class CardUi : Control
{
    [Export] private Label _titleLabel;
    [Export] private Label _costLabel;
    [Export] private RichTextLabel _descriptionLabel;
    [Export] private TextureRect _artTexture;

    /// <summary>
    /// The cost in Scrap required to play this card.
    /// </summary>
    public int PlayCost { get; private set; }

    /// <summary>
    /// The type of wagon this card will create or upgrade.
    /// </summary>
    public WagonType TypeToApply { get; private set; }

    /// <summary>
    /// Static flag to track if any card is currently being dragged.
    /// </summary>
    internal static bool IsAnyCardDragged;

    private bool _isDragging;
    private Vector2 _startPos;

    /// <summary>
    /// Initializes the card's visual elements based on its wagon type.
    /// </summary>
    public void Setup(WagonType type)
    {
        TypeToApply = type;

        if (_titleLabel == null || _costLabel == null) return;

        switch (type)
        {
            case WagonType.Combat:
                _titleLabel.Text = "Turret MK-1";
                PlayCost = 50;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text =
                    "Building / Defense. An automated turret designed to [b]protect[/b] the train.";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Turret-MK1.png");
                break;
            case WagonType.Storage:
                _titleLabel.Text = "Storage";
                PlayCost = 25;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text = "Increases resource capacity.";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Storage.png");
                break;
            case WagonType.Living:
                _titleLabel.Text = "Living Quarters";
                PlayCost = 10;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text = "Provides space for more passengers.";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Living.png");
                break;
            case WagonType.Research:
                _titleLabel.Text = "Research Labs";
                PlayCost = 100;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text = "Generates knowledge over time.";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Research.png");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the visual feedback (color) based on whether the player can afford the card.
    /// </summary>
    public override void _Process(double delta)
    {
        if (GetCurrentScrap() < PlayCost)
        {
            _costLabel.Modulate = new Color(1.0f, 0.3f, 0.3f);
            if (!_isDragging) Modulate = new Color(1f, 0.3f, 0.3f, 0.8f);
        }
        else
        {
            _costLabel.Modulate = new Color(1.0f, 1.0f, 1.0f);
            if (!_isDragging) Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    /// <summary>
    /// Handles mouse input for dragging and dropping the card.
    /// </summary>
    public override void _GuiInput(InputEvent @event)
    {
        var main = GetTree().Root.GetNodeOrNull<Main>("Main");

        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton:
                if (mouseButton.Pressed)
                {
                    if (GetCurrentScrap() < PlayCost) return;

                    // Start dragging.
                    _isDragging = true;
                    IsAnyCardDragged = true;
                    _startPos = GlobalPosition;
                    TopLevel = true;
                    ZIndex = 100;
                    Modulate = new Color(1f, 1f, 1f, 0.4f);
                }
                else if (_isDragging)
                {
                    // Stop dragging and attempt to play the card.
                    _isDragging = false;
                    IsAnyCardDragged = false;
                    ZIndex = 0;
                    Modulate = new Color(1f, 1f, 1f);
                    main?.HidePreview();

                    var success = main != null && main.TryPlayCard(TypeToApply, PlayCost, GetGlobalMousePosition());
                    if (!success)
                    {
                        // Return to hand if play failed.
                        TopLevel = false;
                        GlobalPosition = _startPos;
                    }
                    else
                    {
                        // Successfully played, remove from hand.
                        QueueFree();
                    }
                }
                break;

            case InputEventMouseMotion mouseMotion when _isDragging:
                // Update position and 3D preview while dragging.
                GlobalPosition += mouseMotion.Relative;
                main?.UpdatePreview(TypeToApply, GetGlobalMousePosition());
                break;
        }
    }

    /// <summary>
    /// Helper to get the current scrap amount from the ECS world.
    /// </summary>
    private static int GetCurrentScrap()
    {
        var world = Core.Autoloads.GameWorld.Instance.World;
        return world.Query<ResourceComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<ResourceComponent>(e))
            .Match(r => r.Scrap, () => 0);
    }
}