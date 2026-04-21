using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Shared;

/// <summary>
/// Represents a system that manages resource-related logic within the game, including
/// updating UI elements to reflect resource changes and controlling the state of resource-dependent actions.
/// </summary>
public class ResourceSystem(Label scrapLabel, Button drawButton) : ISystem {
    /// <summary>
    /// Updates the state of the resource system by synchronizing resource data with the UI elements
    /// and enabling or disabling certain UI interactions based on the current resource values.
    /// </summary>
    /// <param name="world">The world instance that contains all entities and components.</param>
    /// <param name="delta">The time elapsed since the last update, typically used for time-based operations.</param>
    public void Update(World world, double delta) {
        Option<Entity> resEntityOpt = world.QueryFirst<ResourceComponent>();
        if (resEntityOpt.IsNone) { return; }

        ref ResourceComponent resources = ref world.Get<ResourceComponent>(resEntityOpt.Unwrap());
        if (scrapLabel != null) { scrapLabel.Text = $"Scrap : {resources.Scrap}"; }

        if (drawButton != null) { drawButton.Disabled = resources.Scrap < ResourceRegistry.CardDrawCost; }
    }
}
