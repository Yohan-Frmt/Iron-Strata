using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Component that stores the current game state.
/// </summary>
public class GameStateComponent : IComponent
{
    public bool IsPaused = false;
    public bool CanPause = true;
}