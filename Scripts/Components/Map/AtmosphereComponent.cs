using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Map;

public class AtmosphereComponent : IComponent
{
    public Color TargetFogColor = new(0.1f, 0.1f, 0.1f);
    public float TargetDensity = 0.05f;
    public float TransitionSpeed = 1.5f;
}