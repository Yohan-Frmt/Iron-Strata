using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Core.Autoloads;

/// <summary>
/// A Godot Autoload (singleton) that hosts the ECS world and system runner.
/// It provides a central access point for the game logic from anywhere in the scene tree.
/// </summary>
public partial class GameWorld : Node
{
    /// <summary>
    /// Singleton instance of the GameWorld node.
    /// </summary>
    public static GameWorld Instance { get; private set; } = null!;

    /// <summary>
    /// The ECS world containing all entities and components.
    /// </summary>
    public World World { get; private set; } = new();

    /// <summary>
    /// The system runner that manages frame and physics updates.
    /// </summary>
    public SystemRunner Runner { get; private set; } = null!;

    /// <summary>
    /// Initializes the singleton and the system runner.
    /// </summary>
    public override void _Ready()
    {
        Instance = this;
        Runner = new SystemRunner(World);
    }

    /// <summary>
    /// Forwards every frame update to the ECS systems.
    /// </summary>
    public override void _Process(double delta) => Runner.Update(delta);

    /// <summary>
    /// Forwards every physics update to the ECS systems.
    /// </summary>
    public override void _PhysicsProcess(double delta) => Runner.FixedUpdate(delta);
}