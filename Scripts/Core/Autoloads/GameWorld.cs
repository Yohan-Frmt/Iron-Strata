using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Core.Autoloads;

public partial class GameWorld : Node
{
    public static GameWorld Instance { get; private set; } = null!;

    public World World { get; private set; } = new();
    public SystemRunner Runner { get; private set; } = null!;

    public override void _Ready()
    {
        Instance = this;
        Runner = new SystemRunner(World);
    }

    public override void _Process(double delta) => Runner.Update(delta);

    public override void _PhysicsProcess(double delta) => Runner.FixedUpdate(delta);
}