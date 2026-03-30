using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Render;

public class RenderableComponent : IComponent
{
    public Node3D Node;
    public Mesh Model;
    public Color Tint = new(0.18f, 0.18f, 0.22f);
    public string Label = "";
}