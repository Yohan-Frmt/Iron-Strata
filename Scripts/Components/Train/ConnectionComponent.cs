using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

public class ConnectionComponent : IComponent
{
    public int PreviousEntityId;
    public int NextEntityId;
    public float Integrity;
    public bool IsWelded;
}