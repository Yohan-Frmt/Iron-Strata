using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Component that defines the coupling between two wagons in the train.
/// </summary>
public class ConnectionComponent : IComponent
{
    /// <summary>
    /// The ID of the wagon ahead of this one.
    /// </summary>
    public int PreviousEntityId;

    /// <summary>
    /// The ID of the wagon behind this one.
    /// </summary>
    public int NextEntityId;

    /// <summary>
    /// The structural integrity of the coupling.
    /// </summary>
    public float Integrity;

    /// <summary>
    /// True if the connection is permanently welded, providing more stability.
    /// </summary>
    public bool IsWelded;
}