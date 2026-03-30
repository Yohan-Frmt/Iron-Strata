using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Map;

public class LocationComponent : IComponent 
{
    public int CurrentNodeId;
    public int TargetNodeId;
    public bool IsInTransit;
    public float TravelProgress;
    public bool IsEditing;
    public bool IsInCityZone;
}