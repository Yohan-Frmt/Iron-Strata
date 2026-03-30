using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Shared;

public class ResourceSystem(Label scrapLabel, Button drawButton) : ISystem
{
    public void Update(World world, double delta)
    {
        var resEntity = world.Query<ResourceComponent>().FirstOrDefault();
        if (resEntity == null) return;
        var resources = world.Get<ResourceComponent>(resEntity);
        if (scrapLabel != null) scrapLabel.Text = $"Scrap : {resources.Scrap}";
        if (drawButton != null) drawButton.Disabled = resources.Scrap < ResourceRegistry.CardDrawCost;
    }
}