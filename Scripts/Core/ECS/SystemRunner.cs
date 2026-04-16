using System.Collections.Generic;
using Godot;

namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// Manages the execution order and lifecycle of ECS systems.
/// It delegates updates to registered frame and physics systems.
/// </summary>
public class SystemRunner(World world)
{
    private readonly List<ISystem> _systems = [];
    private readonly List<IFixedSystem> _fixedSystems = [];

    /// <summary>
    /// Registers a new frame system to be updated every frame.
    /// </summary>
    /// <param name="system">The system instance to add.</param>
    /// <returns>The SystemRunner instance for method chaining.</returns>
    public SystemRunner Add(ISystem system)
    {
        _systems.Add(system);
        return this;
    }

    /// <summary>
    /// Registers a new physics system to be updated at a fixed interval.
    /// </summary>
    /// <param name="system">The system instance to add.</param>
    /// <returns>The SystemRunner instance for method chaining.</returns>
    public SystemRunner Add(IFixedSystem system)
    {
        _fixedSystems.Add(system);
        return this;
    }

    /// <summary>
    /// Updates all registered frame systems.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    public void Update(double delta)
    {
        for (var i = 0; i < _systems.Count; i++)
        {
            _systems[i].Update(world, delta);
        }
    }

    /// <summary>
    /// Updates all registered physics systems.
    /// </summary>
    /// <param name="delta">The fixed time step in seconds.</param>
    public void FixedUpdate(double delta)
    {
        for (var i = 0; i < _fixedSystems.Count; i++)
        {
            _fixedSystems[i].FixedUpdate(world, delta);
        }
    }
}