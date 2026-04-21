namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Represents a component that defines the characteristics of a slot within a wagon
/// in a train system. This struct is primarily used in systems to identify and manage
/// the position and organization of entities associated with train wagons.
/// </summary>
public struct WagonSlotComponent {
    /// <summary>
    /// Represents the index of a slot within a wagon, used to determine and compute
    /// the relative position, behavior, or alignment of entities associated with
    /// specific slots in a train structure.
    /// </summary>
    public int SlotIndex;

    /// <summary>
    /// Represents the vertical layer or level of a wagon slot
    /// within a multi-layered train structure. This variable is used
    /// across various systems to determine or manipulate the placement
    /// and behavior of entities based on their layer within the train.
    /// </summary>
    public int Layer;
}
