using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Shared;

/// <summary>
/// System that handles input for opening the full map.
/// </summary>
public class MapInputSystem : ISystem
{
    public void Update(World world, double delta)
    {
        if (Input.IsActionJustPressed("show_map"))
        {
            var stateEntityOpt = world.QueryFirst<GameStateComponent>();
            if (stateEntityOpt.IsSome)
            {
                ref var state = ref world.Get<GameStateComponent>(stateEntityOpt.Unwrap());
                state.IsMapOpen = !state.IsMapOpen;
            }
        }
    }
}
