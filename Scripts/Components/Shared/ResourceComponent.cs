using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Component that tracks the various resources held by an entity (like the player train).
/// </summary>
public class ResourceComponent : IComponent
{
    /// <summary>
    /// The amount of Scrap (primary currency/material) available.
    /// </summary>
    public int Scrap;
}