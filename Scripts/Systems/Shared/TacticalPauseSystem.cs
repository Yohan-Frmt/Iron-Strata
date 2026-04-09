using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;

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
        var stateEntity = _world.Query<GameStateComponent>().FirstOrDefault();
        if (stateEntity is { IsNull: true }) return;
        var state = _world.Get<GameStateComponent>(stateEntity);
        if (!state.CanPause) return;
        state.IsPaused = !state.IsPaused;
        TogglePause(state.IsPaused);
    }

    private void TogglePause(bool paused)
    {
        Engine.TimeScale = paused ? 0.0f : 1.0f;
        pauseOverlay.Visible = paused;
    }
}