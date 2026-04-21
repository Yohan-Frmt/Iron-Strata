using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Shared;

/// <summary>
/// Handles the tactical pause functionality within the game, allowing players
/// to pause and resume gameplay dynamically via a specified input action.
/// </summary>
/// <remarks>
/// This system monitors input for the "pause_tactical" action and toggles
/// the game's paused state accordingly. It integrates with the game world
/// to manage pause-related state and ensures proper interaction with game entities.
/// This class is designed to interact specifically with game components such
/// as <c>GameStateComponent</c>.
/// </remarks>
public class TacticalPauseSystem(Control pauseOverlay) : ISystem {
    /// <summary>
    /// Represents the game world that provides entity management, component storage,
    /// and various querying capabilities for interacting with entities and their associated data.
    /// </summary>
    private World _world;

    /// <summary>
    /// Processes the game's tactical pause functionality by monitoring input
    /// and invoking the pause state toggle when the relevant input action is detected.
    /// Updates the internal reference to the game world for system interactions.
    /// </summary>
    /// <param name="world">The game world containing the entities and components relevant to system operations.</param>
    /// <param name="delta">The frame time elapsed since the last update, used for time-dependent calculations.</param>
    public void Update(World world, double delta) {
        _world = world;
        if (Input.IsActionJustPressed("pause_tactical")) { TriggerPause(); }
    }

    /// <summary>
    /// Toggles the tactical pause state of the game by modifying the pause state
    /// within the associated GameStateComponent. If pausing is allowed, the method
    /// updates the paused state and invokes the corresponding visual or functional
    /// adjustments to reflect the change.
    /// </summary>
    public void TriggerPause() {
        Option<Entity> stateEntityOpt = _world.QueryFirst<GameStateComponent>();
        if (stateEntityOpt.IsNone) { return; }

        ref GameStateComponent state = ref _world.Get<GameStateComponent>(stateEntityOpt.Unwrap());
        if (!state.CanPause) { return; }

        state.IsPaused = !state.IsPaused;
        TogglePause(state.IsPaused);
    }

    /// <summary>
    /// Toggles the game's paused state by adjusting the time scale and
    /// visibility of the pause overlay.
    /// </summary>
    /// <param name="isPaused">A boolean indicating whether the game is paused.
    /// If true, the game will be paused; otherwise, it will be unpaused.</param>
    private void TogglePause(bool isPaused) {
        Engine.TimeScale = isPaused ? 0.0f : 1.0f;
        if (pauseOverlay != null) { pauseOverlay.Visible = isPaused; }
    }
}
