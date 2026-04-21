namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Represents a resource component within the game, tracking resource-specific data.
/// Used by systems such as `ResourceSystem` and `TurretSystem` to manage and update resource-related game state.
/// </summary>
public struct ResourceComponent {
    /// <summary>
    /// Represents the current amount of scrap, a resource used within the game's economy.
    /// Scrap is consumed or modified by various systems such as <c>ResourceSystem</c>,
    /// <c>TurretSystem</c>, and <c>ConstructionSystem</c>.
    /// </summary>
    public int Scrap;
}
