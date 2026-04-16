using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Shared;

public class TacticalPauseSystem(Control pauseOverlay) : ISystem
{
    private World _world;

    public void Update(World world, double delta)
    {
        _world = world;
        if (Input.IsActionJustPressed("pause_tactical")) TriggerPause();
    }

    public void TriggerPause()
    {
        var stateEntityOpt = _world.QueryFirst<GameStateComponent>();
        if (stateEntityOpt.IsNone) return;
        ref var state = ref _world.Get<GameStateComponent>(stateEntityOpt.Unwrap());
        if (!state.CanPause) return;
        state.IsPaused = !state.IsPaused;
        TogglePause(state.IsPaused);
    }

    private void TogglePause(bool isPaused)
    {
        Engine.TimeScale = isPaused ? 0.0f : 1.0f;
        if (pauseOverlay != null) pauseOverlay.Visible = isPaused;
    }
}