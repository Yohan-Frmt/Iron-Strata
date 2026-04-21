namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Represents a component that defines the current state of the game,
/// including properties for pausing the game, toggling the state of pause functionality,
/// and managing the visibility of the map overlay.
/// </summary>
public struct GameStateComponent() {
    /// <summary>
    /// Indicates whether the game is currently paused.
    /// When set to true, the game's time progression and gameplay mechanics are halted.
    /// When false, the game resumes normal operation.
    /// </summary>
    public bool IsPaused = false;

    /// <summary>
    /// Determines whether pausing the game is currently allowed.
    /// When set to true, the game can be paused by user input or other systems.
    /// When false, all requests to pause the game are ignored.
    /// </summary>
    public bool CanPause = true;

    /// <summary>
    /// Indicates whether the map overlay is currently visible in the game.
    /// When set to true, the map is displayed, providing a broader view or tactical information.
    /// When false, the map overlay is hidden, allowing unobstructed gameplay visuals.
    /// </summary>
    public bool IsMapOpen = false;
}
