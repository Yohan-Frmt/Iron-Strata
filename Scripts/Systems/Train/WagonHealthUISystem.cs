using Godot;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;

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
            
            if (render.Node == null) continue;

            // Find or create the HP label attached to the wagon node.
            var hpLabel = render.Node.GetNodeOrNull<Label3D>("HPLabel");
            if (hpLabel == null)
            {
                hpLabel = new Label3D { Name = "HPLabel" };
                hpLabel.Position = new Vector3(0, 3f, 0);
                hpLabel.PixelSize = 0.03f;
                hpLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
                hpLabel.OutlineRenderPriority = 0;
                hpLabel.OutlineSize = 6;
                hpLabel.FontSize = 40;
                render.Node.AddChild(hpLabel);
            }

            // Update text and color based on health status.
            hpLabel.Text = $"{(int)health.Current} / {(int)health.Max}";
            
            // Turn red if health is low.
            hpLabel.Modulate = health.Current < health.Max * 0.3f 
                ? new Color(1.0f, 0.2f, 0.2f) 
                : new Color(0.8f, 1.0f, 0.8f);
        }
    }
}