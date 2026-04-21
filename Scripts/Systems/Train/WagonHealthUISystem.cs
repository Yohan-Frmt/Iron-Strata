using Godot;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// The WagonHealthUiSystem is responsible for managing and updating the UI elements
/// that display the health status of wagon entities within the game. It ensures that
/// each wagon entity with a health component has a corresponding visual representation
/// of its health in the world.
/// </summary>
public class WagonHealthUiSystem : ISystem {
    /// <summary>
    /// Updates the health display of wagon entities in the game world. This method iterates through all entities
    /// that contain both a HealthComponent and a RenderableComponent, ensuring their associated UI labels are
    /// updated to reflect their current health status. If a health label does not exist for an entity, one is created
    /// and configured. The text and color of the label are adjusted based on the entity's health.
    /// </summary>
    /// <param name="world">The game world containing entities and components to process.</param>
    /// <param name="delta">The time elapsed since the last update, used for temporal calculations if needed.</param>
    public void Update(World world, double delta) {
        foreach (Entity entity in world.Query<HealthComponent, RenderableComponent>()) {
            ref readonly HealthComponent health = ref world.Get<HealthComponent>(entity);
            ref readonly RenderableComponent render = ref world.Get<RenderableComponent>(entity);

            if (render.Node == null) { continue; }

            Node3D node = render.Node;
            Label3D hpLabel = node.GetNodeOrNull<Label3D>("HPLabel");
            if (hpLabel == null) {
                hpLabel = new Label3D {
                    Name = "HPLabel",
                    Position = new Vector3(0, 3f, 0),
                    PixelSize = 0.03f,
                    Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
                    OutlineRenderPriority = 0,
                    OutlineSize = 6,
                    FontSize = 40
                };
                node.AddChild(hpLabel);
            }

            hpLabel.Text = $"{(int)health.Current} / {(int)health.Max}";

            hpLabel.Modulate = health.Current < health.Max * 0.3f
                ? new Color(1.0f, 0.2f, 0.2f)
                : new Color(0.8f, 1.0f, 0.8f);
        }
    }
}
