using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

public enum WagonType { Locomotive, Living, Combat, Storage, Research, Medical }

public partial class WagonTypeComponent : IComponent
{
    public WagonType Type;
    public string BlueprintId;
}