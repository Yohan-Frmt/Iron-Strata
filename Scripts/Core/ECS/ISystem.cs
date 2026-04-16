namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// Interface for systems that run on every frame update.
/// Systems contain logic that operates on entities with specific components.
/// </summary>
public interface ISystem
{
    /// <summary>
    /// Processes the game logic for the current frame.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    void Update(World world, double delta);
}

/// <summary>
/// Interface for systems that run on every physics update.
/// </summary>
public interface IFixedSystem
{
    /// <summary>
    /// Processes physics-related logic at a fixed interval.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The fixed time step in seconds.</param>
    void FixedUpdate(World world, double delta);
}