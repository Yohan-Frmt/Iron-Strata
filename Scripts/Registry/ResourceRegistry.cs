namespace IronStrata.Scripts.Registry;

/// <summary>
/// Represents a centralized registry for managing constant values related to resource management across systems.
/// The constants defined in this class serve as reference points for implementing various gameplay mechanics
/// such as costs, rewards, and starting conditions.
/// </summary>
public static class ResourceRegistry {
    /// <summary>
    /// Represents the default cost of a wagon in scrap currency.
    /// </summary>
    public const int DefaultWagonCost = 25;

    /// <summary>
    /// Represents the amount of scrap rewarded for defeating an enemy.
    /// This constant value is used to define how much scrap is added to the player's resources
    /// each time an enemy is defeated in the game.
    /// </summary>
    public const int ScrapPerKill = 5;

    /// <summary>
    /// Represents the number of Scrap required to draw a single card.
    /// This value is used as the cost for reducing the player's Scrap currency
    /// when they attempt to draw a card in the game. The player must have at least
    /// this amount of Scrap to perform the card draw operation.
    /// </summary>
    public const int CardDrawCost = 10;

    /// <summary>
    /// Represents the initial amount of scrap resources available in the system.
    /// This is used as the starting value for tracking scrap resource availability.
    /// </summary>
    public const int StartingScrap = 0;
}
