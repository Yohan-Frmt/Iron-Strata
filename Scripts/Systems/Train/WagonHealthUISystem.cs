using Godot;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// System that creates and updates floating health bars (Label3D) above train wagons.
/// </summary>
public class WagonHealthUiSystem : ISystem
{
    /// <summary>
    /// Processes and updates the health labels for all wagons.
    /// </summary>
    public void Update(World world, double delta)
    {
        foreach (var entity in world.Query<HealthComponent, RenderableComponent>())
        {
            var health = world.Get<HealthComponent>(entity);
            var render = world.Get<RenderableComponent>(entity);
            
            render.Node.ToOption().Match(node => 
            {
                // Find or create the HP label attached to the wagon node.
                var hpLabel = node.GetNodeOrNull<Label3D>("HPLabel").ToOption().UnwrapOrElse(() => 
                {
                    var newLabel = new Label3D { Name = "HPLabel" };
                    newLabel.Position = new Vector3(0, 3f, 0);
                    newLabel.PixelSize = 0.03f;
                    newLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
                    newLabel.OutlineRenderPriority = 0;
                    newLabel.OutlineSize = 6;
                    newLabel.FontSize = 40;
                    node.AddChild(newLabel);
                    return newLabel;
                });

                // Update text and color based on health status.
                hpLabel.Text = $"{(int)health.Current} / {(int)health.Max}";
                
                // Turn red if health is low.
                hpLabel.Modulate = health.Current < health.Max * 0.3f 
                    ? new Color(1.0f, 0.2f, 0.2f) 
                    : new Color(0.8f, 1.0f, 0.8f);
            }, () => { });
        }
    }
}