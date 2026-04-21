using System.Collections.Generic;

namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// Manages the execution order and lifecycle of ECS systems.
/// It delegates updates to registered frame and physics systems.
/// </summary>
public class SystemRunner(World world) {
    private readonly List<ISystem> _systems = [];
    private readonly List<IFixedSystem> _fixedSystems = [];

    /// <summary>
    /// Registers a new frame system to be updated every frame.
    /// </summary>
    /// <param name="system">The system instance to add.</param>
    /// <returns>The SystemRunner instance for method chaining.</returns>
    public SystemRunner Add(ISystem system) {
        _systems.Add(system);
        return this;
    }

    /// <summary>
    /// Registers a new physics system to be updated at a fixed interval.
    /// </summary>
    /// <param name="system">The system instance to add.</param>
    /// <returns>The SystemRunner instance for method chaining.</returns>
    public SystemRunner Add(IFixedSystem system) {
        _fixedSystems.Add(system);
        return this;
    }

    /// <summary>
    /// Updates all registered frame systems with the specified time delta.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    public void Update(double delta) {
        for (int systemIndex = 0; systemIndex < _systems.Count; systemIndex++) { _systems[systemIndex].Update(world, delta); }
    }

    /// <summary>
    /// Updates all registered fixed systems with the provided fixed time step.
    /// </summary>
    /// <param name="delta">The fixed time step in seconds.</param>
    public void FixedUpdate(double delta) {
        for (int systemIndex = 0; systemIndex < _fixedSystems.Count; systemIndex++) { _fixedSystems[systemIndex].FixedUpdate(world, delta); }
    }
}
