namespace IronStrata.Scripts.Registry;

/// <summary>
/// Static registry for balancing and economy constants.
/// </summary>
public static class ResourceRegistry
{
    /// <summary>
    /// Base cost in Scrap to build a new wagon or upgrade an existing one.
    /// </summary>
    public const int DefaultWagonCost = 25;

    /// <summary>
    /// Amount of Scrap rewarded for destroying a silicon lifeform.
    /// </summary>
    public const int ScrapPerKill = 5;

    /// <summary>
    /// Cost in Scrap to draw a new wagon card from the deck.
    /// </summary>
    public const int CardDrawCost = 10;

    /// <summary>
    /// Amount of Scrap the player starts with at the beginning of a run.
    /// </summary>
    public const int StartingScrap = 0;
}