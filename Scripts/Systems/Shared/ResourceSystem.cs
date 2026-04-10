using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Shared;

/// <summary>
/// System that synchronizes resource data with the UI.
/// </summary>
public class ResourceSystem(Label scrapLabel, Button drawButton) : ISystem
{
    /// <summary>
    /// Updates UI elements related to resources, such as the Scrap counter and draw button state.
    /// </summary>
    public void Update(World world, double delta)
    {
        world.Query<ResourceComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<ResourceComponent>(e))
            .Match(resources => 
            {
                // Update the scrap counter text.
                if (scrapLabel != null) 
                    scrapLabel.Text = $"Scrap : {resources.Scrap}";
                
                // Disable the draw button if the player can't afford it.
                if (drawButton != null) 
                    drawButton.Disabled = resources.Scrap < ResourceRegistry.CardDrawCost;
            }, () => { });
    }
}