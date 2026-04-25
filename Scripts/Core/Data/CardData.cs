using Godot;
using IronStrata.Scripts.Components.Train;

namespace IronStrata.Scripts.Core.Data;

/// <summary>
/// Defines the classification of cards in the game.
/// </summary>
public enum CardType { Wagon, Action }

/// <summary>
/// Defines the types of actions an action card can perform.
/// </summary>
public enum ActionType { Heal, BoostRange, AddScrap, DrawCard }

/// <summary>
/// Data resource defining a playable card, which can be either a wagon or an action.
/// </summary>
[GlobalClass]
public partial class CardData : Resource {
    /// <summary>
    /// The display title of the card.
    /// </summary>
    [Export]
    public string Title { get; set; } = "New Card";

    /// <summary>
    /// The description of the card's effect.
    /// </summary>
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = "";

    /// <summary>
    /// The artwork displayed on the card.
    /// </summary>
    [Export]
    public Texture2D Art { get; set; }

    /// <summary>
    /// The cost in scrap required to play this card.
    /// </summary>
    [Export]
    public int PlayCost { get; set; } = 0;

    /// <summary>
    /// The type of the card (Wagon or Action).
    /// </summary>
    [Export]
    public CardType Type { get; set; } = CardType.Wagon;

    [ExportGroup("Wagon Properties")]
    /// <summary>
    /// The type of wagon this card creates (if Type is Wagon).
    /// </summary>
    [Export]
    public WagonType WagonTypeToApply { get; set; } = WagonType.Storage;

    [ExportGroup("Action Properties")]
    /// <summary>
    /// The type of action this card performs (if Type is Action).
    /// </summary>
    [Export]
    public ActionType ActionType { get; set; } = ActionType.Heal;

    /// <summary>
    /// The numerical value associated with the action (e.g., amount of HP, range boost, or scrap).
    /// </summary>
    [Export]
    public float ActionValue { get; set; } = 0f;
}
