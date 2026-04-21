using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Core.Autoloads;

/// <summary>
/// Represents the core game world, serving as the main entry point for key game systems and resources.
/// </summary>
/// <remarks>
/// This class provides centralized management of the game state, using an ECS architecture
/// via its <see cref="SystemRunner"/>. It is responsible for initializing and running game logic systems
/// and ensuring communication between them.
/// </remarks>
public partial class GameWorld : Node {
    /// <summary>
    /// Gets the singleton instance of the <see cref="GameWorld"/> class,
    /// providing centralized access to key game systems and resources.
    /// </summary>
    /// <remarks>
    /// The <c>Instance</c> property serves as the globally accessible entry point
    /// to the <see cref="GameWorld"/> class, ensuring that all components and systems
    /// requiring a reference to the game world can access it reliably.
    /// It is assigned during the <c>_Ready</c> method execution and remains constant throughout
    /// the application's lifecycle. Proper initialization of this property is critical to
    /// ensure functionality in dependent classes such as <see cref="IronStrata.Scripts.UI.Minimap"/>,
    /// <see cref="IronStrata.Scripts.UI.CardUi"/>, and <see cref="IronStrata.Scripts.UI.MapOverlay"/>.
    /// </remarks>
    public static GameWorld Instance { get; private set; } = null!;

    /// <summary>
    /// Gets the instance of the <see cref="World"/> used for managing
    /// and maintaining the entity-component-system (ECS) structure within the game.
    /// </summary>
    /// <remarks>
    /// The <c>World</c> property serves as the core data structure for the ECS framework,
    /// providing functionalities for creating, managing, and interacting with entities
    /// and their associated components. It is initialized during the <c>_Ready</c> method
    /// of the <see cref="GameWorld"/> and is accessed by various game systems and UI components.
    /// </remarks>
    public World World { get; } = new();

    /// <summary>
    /// Gets the instance of the <see cref="SystemRunner"/> used for managing
    /// and executing systems within the game world's ECS framework.
    /// </summary>
    /// <remarks>
    /// The <c>Runner</c> property is a core component of the ECS infrastructure,
    /// responsible for updating and managing the lifecycle of systems.
    /// It is initialized during the <c>_Ready</c> method of the <c>GameWorld</c>.
    /// </remarks>
    public SystemRunner Runner { get; private set; } = null!;

    /// <summary>
    /// Called when the node is added to the scene tree for initialization purposes.
    /// Sets up the singleton instance of <see cref="GameWorld"/> and prepares the
    /// <see cref="SystemRunner"/> instance for managing ECS systems.
    /// </summary>
    public override void _Ready() {
        Instance = this;
        Runner = new SystemRunner(World);
    }

    /// <summary>
    /// Called every frame to handle general game logic updates.
    /// Delegates the frame update to the <see cref="SystemRunner"/> for processing registered frame systems.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    public override void _Process(double delta) => Runner.Update(delta);

    /// <summary>
    /// Called every physics frame with a fixed time step to handle physics-related updates.
    /// Delegates the fixed time step to the <see cref="SystemRunner"/> for processing registered physics systems.
    /// </summary>
    /// <param name="delta">The fixed time step in seconds.</param>
    public override void _PhysicsProcess(double delta) => Runner.FixedUpdate(delta);
}
