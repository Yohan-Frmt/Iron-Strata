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
    /// Updates the train's movement and its associated heads-up display by managing physics calculations
    /// and reflecting changes in the UI based on the current train state.
    /// </summary>
    /// <param name="world">The game world containing the train's entities and components.</param>
    /// <param name="delta">The elapsed time since the last frame, used for physics updates.</param>
    public void Update(World world, double delta)
    {
        var entityOpt = world.QueryFirst<TrainMovementComponent>();
        if (entityOpt.IsNone) return;
        var entity = entityOpt.Unwrap();
        ref var movement = ref world.Get<TrainMovementComponent>(entity);
        UpdatePhysics(ref movement, (float)delta);
        UpdateHud(movement);
    }

    /// <summary>
    /// Updates the physics properties of the train's movement, including speed, braking, and distance traveled.
    /// </summary>
    /// <param name="movement">The movement component containing the train's physics state, including speed, acceleration, deceleration, braking, and distance traveled.</param>
    /// <param name="delta">The time elapsed since the last update, used to calculate changes in physics properties.</param>
    private static void UpdatePhysics(ref TrainMovementComponent movement, float delta)
    {
        movement.IsBraking = Input.IsActionPressed("ui_accept");
        var targetSpeed = movement.IsBraking ? 0f : movement.MaxSpeed;
        var accelRate = movement.IsBraking ? movement.Deceleration : movement.Acceleration;
        movement.Speed = Mathf.MoveToward(movement.Speed, targetSpeed, accelRate * delta);
        movement.DistanceTraveled += movement.Speed * delta;
    }

    /// <summary>
    /// Updates the heads-up display (HUD) to reflect the current state of the train's movement,
    /// including speed, distance traveled, and braking status.
    /// </summary>
    /// <param name="movement">The train's movement component containing speed, distance, and braking information.</param>
    private void UpdateHud(TrainMovementComponent movement)
    {
        var distanceStr = $"{movement.DistanceTraveled / 1000f:F1} km";
        speedLabel.Text = movement.IsBraking ? $"▸ {movement.Speed:F0} u/s   {distanceStr}   ▌▌ BRAKE" : $"▸ {movement.Speed:F0} u/s   {distanceStr}";
    }
}