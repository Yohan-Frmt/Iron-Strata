namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// Interface for defining systems within an Entity Component System (ECS) architecture.
/// Systems implementing this interface are responsible for processing game logic by
/// interacting with the game world and its entities during each frame update.
/// </summary>
public interface ISystem {
    /// <summary>
    /// Processes the game logic for the current frame.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    void Update(World world, double delta);
}

/// <summary>
/// Interface for systems that execute physics logic at fixed intervals.
/// Fixed systems operate independently of the frame rate and are synchronized
/// with the fixed time step defined by the game engine.
/// </summary>
public interface IFixedSystem {
    /// <summary>
    /// Processes physics-related logic at a fixed interval.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The fixed time step in seconds.</param>
    void FixedUpdate(World world, double delta);
}
