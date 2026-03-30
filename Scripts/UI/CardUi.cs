using System;
using System.Linq;
using Godot;
using IronStrata.Scenes;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;

namespace IronStrata.Scripts.UI;

public partial class CardUi : Control
{
    [Export] private Label _titleLabel;
    [Export] private Label _costLabel;
    [Export] private RichTextLabel _descriptionLabel;
    [Export] private TextureRect _artTexture;

    public int PlayCost { get; private set; }
    public WagonType TypeToApply { get; private set; }
    internal static bool IsAnyCardDragged;
    private bool _isDragging;
    private Vector2 _startPos;

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
                    "Bâtiment / Défense. Une tourelle automatisée conçue pour [b]protéger[/b] le train.";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Turret-MK1.png");
                break;
            case WagonType.Storage:
                _titleLabel.Text = "Storage";
                PlayCost = 25;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text = "";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Storage.png");
                break;
            case WagonType.Living:
                _titleLabel.Text = "Living Room";
                PlayCost = 10;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text = "";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Living.png");
                break;
            case WagonType.Research:
                _titleLabel.Text = "Research Labs";
                PlayCost = 100;
                _costLabel.Text = PlayCost.ToString();
                _descriptionLabel.Text = "";
                _artTexture.Texture = GD.Load<Texture2D>("res://Resources/Assets/Images/Cards/Wagons/Research.png");
                break;
            case WagonType.Locomotive:
            case WagonType.Medical:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

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

    public override void _GuiInput(InputEvent @event)
    {
        var main = GetTree().Root.GetNodeOrNull<Main>("Main");

        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton:
                switch (mouseButton.Pressed)
                {
                    case true when GetCurrentScrap() < PlayCost:
                        return;
                    case true:
                        {
                            _isDragging = true;
                            IsAnyCardDragged = true;
                            var exactGlobalPos = GlobalPosition;
                            _startPos = exactGlobalPos;
                            TopLevel = true;
                            ZIndex = 100;
                            GlobalPosition = exactGlobalPos;
                            Modulate = new Color(1f, 1f, 1f, 0.4f);
                            break;
                        }
                    case false when _isDragging:
                        {
                            _isDragging = false;
                            IsAnyCardDragged = false;
                            ZIndex = 0;
                            Modulate = new Color(1f, 1f, 1f);
                            main?.HidePreview();

                            var success = main != null && main.TryPlayCard(TypeToApply, PlayCost, GetGlobalMousePosition());
                            if (!success)
                            {
                                TopLevel = false;
                                GlobalPosition = _startPos;
                            }
                            else
                            {
                                QueueFree();
                            }

                            break;
                        }
                }

                break;
            case InputEventMouseMotion mouseMotion when _isDragging:
                GlobalPosition += mouseMotion.Relative;
                main?.UpdatePreview(TypeToApply, GetGlobalMousePosition());
                break;
        }
    }

    private static int GetCurrentScrap()
    {
        var world = Core.Autoloads.GameWorld.Instance.World;
        var resEntity = world.Query<ResourceComponent>().FirstOrDefault();
        return resEntity != null ? world.Get<ResourceComponent>(resEntity).Scrap : 0;
    }
}