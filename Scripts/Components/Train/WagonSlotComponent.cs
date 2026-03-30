using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Component that defines the spatial position of a wagon within the train's logical grid.
/// </summary>
public class WagonSlotComponent : IComponent
{
    /// <summary>
    /// The horizontal index in the train layout (how far from the locomotive).
    /// </summary>
    public int SlotIndex;

    /// <summary>
    /// The vertical layer or height in a stacked train configuration.
    /// </summary>
    public int Layer;
}