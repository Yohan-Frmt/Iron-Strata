using System.Linq;
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
        _world.Query<GameStateComponent>()
            .FirstOptional()
            .Bind(_world.GetOptional<GameStateComponent>)
            .Match(state =>
            {
                if (!state.CanPause) return;
                state.IsPaused = !state.IsPaused;
                TogglePause(state.IsPaused);
            });
    }

    private void TogglePause(bool paused)
    {
        Engine.TimeScale = paused ? 0.0f : 1.0f;
        pauseOverlay.Visible = paused;
    }
}