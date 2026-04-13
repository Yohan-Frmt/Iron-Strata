using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// System that manages the locomotive's speed and the camera's behavior.
/// It handles inputs for braking, zooming, and manual camera panning.
/// </summary>
public class TrainMovementSystem(Label speedLabel) : ISystem
{
    /// <summary>
    /// Handles movement logic, camera interpolation, and UI updates.
    /// </summary>
    public void Update(World world, double delta)
    {
        world.Query<TrainMovementComponent>()
            .FirstOptional()
            .Match(entity => 
            {
                var movement = world.Get<TrainMovementComponent>(entity);
                UpdatePhysics(movement, (float)delta);
                UpdateHud(movement);
            });
    }

    /// <summary>
    /// Updates the physics of the train by adjusting its speed, applying acceleration or deceleration,
    /// and calculating the distance traveled based on the current speed.
    /// </summary>
    /// <param name="movement">The movement component containing the train's physics properties such as speed, acceleration, and braking status.</param>
    /// <param name="delta">The frame time delta used to calculate the updated physics values.</param>
    private static void UpdatePhysics(TrainMovementComponent movement, float delta)
    {
        movement.IsBraking = Input.IsActionPressed("ui_accept");

        var targetSpeed = movement.IsBraking ? 0f : movement.MaxSpeed;
        var accelRate = movement.IsBraking ? movement.Deceleration : movement.Acceleration;
        
        movement.Speed = Mathf.MoveToward(movement.Speed, targetSpeed, accelRate * delta);
        movement.DistanceTraveled += movement.Speed * delta;
    }

    private void UpdateHud(TrainMovementComponent movement)
    {
        var distanceStr = $"{movement.DistanceTraveled / 1000f:F1} km";
        speedLabel.Text = movement.IsBraking ? $"▸ {movement.Speed:F0} u/s   {distanceStr}   ▌▌ BRAKE" : $"▸ {movement.Speed:F0} u/s   {distanceStr}";
    }
}