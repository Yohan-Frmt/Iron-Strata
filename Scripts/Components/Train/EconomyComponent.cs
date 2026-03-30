using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

public class EconomyComponent : IComponent
{
    public float Rations = 1000f;
    public float BaseConsumption = 2.0f;
}