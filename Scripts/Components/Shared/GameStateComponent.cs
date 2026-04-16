using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Component that stores the current game state.
/// </summary>
public struct GameStateComponent()
{
    public bool IsPaused = false;
    public bool CanPause = true;
    public bool IsMapOpen = false;
}