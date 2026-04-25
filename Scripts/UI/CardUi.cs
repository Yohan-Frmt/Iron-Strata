using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.UI;

/// <summary>
/// Controls the behavior and visual representation of a wagon card in the player's hand.
/// Handles drag-and-drop interactions for construction.
/// </summary>
public partial class CardUi : Control {
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

    /// <summary>
    /// Indicates whether the player is currently dragging the card during a drag-and-drop interaction.
    /// Used to modify visual properties and handle positional updates while the card is being manipulated.
    /// </summary>
    private bool _isDragging;

    /// <summary>
    /// Stores the initial global position of the card before a drag-and-drop interaction begins.
    /// Used to restore the card's position if the interaction is canceled or unsuccessful.
    /// </summary>
    private Vector2 _startPosition;

    /// <summary>
    /// Configures the card's appearance and data based on the specified wagon type.
    /// Sets the title, play cost, description, and associated artwork to reflect the wagon type.
    /// </summary>
    /// <param name="type">The type of wagon to configure this card for.</param>
    public void Setup(WagonType type) {
        TypeToApply = type;

        if (_titleLabel == null || _costLabel == null) {
            return;
        }

        switch (type) {
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
            case WagonType.Locomotive:
            case WagonType.Medical:
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the visual state of the card based on the current scrap amount and play cost,
    /// adjusting the color of the cost label and card to indicate affordability.
    /// </summary>
    /// <param name="delta">The frame time elapsed, used for frame-based updates.</param>
    public override void _Process(double delta) {
        if (GetCurrentScrap() < PlayCost) {
            _costLabel.Modulate = new Color(1.0f, 0.3f, 0.3f);
            if (!_isDragging) {
                Modulate = new Color(1f, 0.3f, 0.3f, 0.8f);
            }
        }
        else {
            _costLabel.Modulate = new Color(1.0f, 1.0f, 1.0f);
            if (!_isDragging) {
                Modulate = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// Stores the offset between the mouse click position and the card's top-left corner.
    /// Used to keep the card exactly under the mouse during drag.
    /// </summary>
    private Vector2 _dragOffset;

    /// <summary>
    /// Handles user input specific to the card UI, including drag-and-drop interactions and card play attempts.
    /// Processes mouse button clicks and movements to manage dragging state, visual feedback, and card placement logic.
    /// </summary>
    /// <param name="event">The input event to process, typically mouse button or motion events.</param>
    public override void _GuiInput(InputEvent @event) {
        Main main = GetTree().Root.GetNodeOrNull<Main>("Main");

        switch (@event) {
            case InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton:
                if (mouseButton.Pressed) {
                    if (GetCurrentScrap() < PlayCost) {
                        return;
                    }

                    _isDragging = true;
                    IsAnyCardDragged = true;
                    _startPosition = GlobalPosition;
                    _dragOffset = GetGlobalMousePosition() - GlobalPosition;
                    TopLevel = true;
                    ZIndex = 100;
                    Modulate = new Color(1f, 1f, 1f, 0.4f);
                }
                else if (_isDragging) {
                    _isDragging = false;
                    IsAnyCardDragged = false;
                    ZIndex = 0;
                    Modulate = new Color(1f, 1f, 1f);
                    main?.HidePreview();

                    bool success = main != null && main.TryPlayCard(TypeToApply, PlayCost, GetGlobalMousePosition()).IsOk;
                    if (!success) {
                        TopLevel = false;
                        GlobalPosition = _startPosition;
                    }
                    else {
                        QueueFree();
                    }
                }
                break;

            case InputEventMouseMotion mouseMotion when _isDragging:
                GlobalPosition = GetGlobalMousePosition() - _dragOffset;
                main?.UpdatePreview(TypeToApply, GetGlobalMousePosition());
                break;
        }
    }

    /// <summary>
    /// Retrieves the current amount of scrap available from the game's resource system.
    /// Queries the world for a resource entity containing the scrap information
    /// and returns the retrieved value or 0 if no scrap data is available.
    /// </summary>
    /// <returns>The current amount of scrap, or 0 if not found.</returns>
    private static int GetCurrentScrap() {
        World world = Core.Autoloads.GameWorld.Instance.World;
        Option<Entity> resEntityOpt = world.QueryFirst<ResourceComponent>();
        return resEntityOpt.IsSome ? world.Get<ResourceComponent>(resEntityOpt.Unwrap()).Scrap : 0;
    }
}
