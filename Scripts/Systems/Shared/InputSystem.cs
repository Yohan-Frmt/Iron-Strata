using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Shared;

/// <summary>
/// Represents a system that processes input events and integrates them into the game's state.
/// </summary>
public class InputSystem : ISystem {
    /// <summary>
    /// Updates the input system by handling input events and modifying the game state accordingly.
    /// </summary>
    /// <param name="world">The simulation world containing entities, components, and systems.</param>
    /// <param name="delta">The elapsed time in seconds since the last update call.</param>
    public void Update(World world, double delta) {
        Option<Entity> stateOption = world.QueryFirst<GameStateComponent>();
        if (stateOption.IsNone) { return; }

        ref GameStateComponent state = ref world.Get<GameStateComponent>(stateOption.Unwrap());

        if (Input.IsActionJustPressed("show_map")) { state.IsMapOpen = !state.IsMapOpen; }

        // For now inversed to comply to qodana
        if (!Input.IsActionJustPressed("pause_tactical") || !state.CanPause) { return; }

        state.IsPaused = !state.IsPaused;
        Engine.TimeScale = state.IsPaused ? 0.0f : 1.0f;
    }
}
