using System.Collections.Generic;

namespace IronStrata.Scripts.Core.ECS;

public class SystemRunner(World world)
{
    private readonly List<ISystem> _systems = [];
    private readonly List<IFixedSystem> _fixedSystems = [];

    public SystemRunner Add(ISystem system)
    {
        _systems.Add(system);
        return this;
    }

    public SystemRunner Add(IFixedSystem system)
    {
        _fixedSystems.Add(system);
        return this;
    }

    public void Update(double delta)
    {
        foreach (var system in _systems) system.Update(world, delta);
    }

    public void FixedUpdate(double delta)
    {
        foreach (var system in _fixedSystems) system.FixedUpdate(world, delta);
    }
    
}