using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Train;

public class TrainMovementSystem : ISystem
{
    private readonly Node3D _trainRoot;
    private readonly Camera3D _camera;
    private readonly Label _speedLabel;
    private const float CameraLerpSpeed = 6f;
    private readonly Vector3 _cameraOffset = new(-20f, 25f, 25f);
    private float _targetZoom = 75f;
    private const float MinZoom = 15f;
    private const float MaxZoom = 150f;
    private float _manualOffset = 0f;
    private Vector2 _lastMousePos;

    public TrainMovementSystem(Node3D trainRoot, Camera3D camera, Label speedLabel)
    {
        _trainRoot = trainRoot;
        _camera = camera;
        _speedLabel = speedLabel;
        _camera.Position = _cameraOffset;
        _camera.LookAt(Vector3.Zero, Vector3.Up);
    }

    public void Update(World world, double delta)
    {
        if (Input.IsActionJustReleased("zoom_in")) _targetZoom -= 4f;
        if (Input.IsActionJustReleased("zoom_out")) _targetZoom += 4f;
        _targetZoom = Mathf.Clamp(_targetZoom, MinZoom, MaxZoom);
        _camera.Size = Mathf.Lerp(_camera.Size, _targetZoom, (float)(10f * delta));

        var currentMousePos = _camera.GetViewport().GetMousePosition();
        if (Input.IsMouseButtonPressed(MouseButton.Left) && !UI.CardUi.IsAnyCardDragged)
        {
            _manualOffset -= (currentMousePos.X - _lastMousePos.X) * 0.15f;
        }
        _lastMousePos = currentMousePos;

        var maxSlot = world.Query<WagonSlotComponent>().Select(e => world.Get<WagonSlotComponent>(e).SlotIndex).Prepend(0).Max();
        var backLimit = -maxSlot * (TrainLayout.WagonLength + TrainLayout.WagonGap);
        _manualOffset = Mathf.Clamp(_manualOffset, backLimit - 5f, 15f);

        foreach (var entity in world.Query<TrainMovementComponent>())
        {
            var mv = world.Get<TrainMovementComponent>(entity);
            mv.IsBraking = Input.IsActionPressed("ui_accept");

            mv.Speed = mv.IsBraking
                ? Mathf.MoveToward(mv.Speed, 0f, mv.Deceleration * (float)delta)
                : Mathf.MoveToward(mv.Speed, mv.MaxSpeed, mv.Acceleration * (float)delta);

            mv.DistanceTraveled += mv.Speed * (float)delta;

            var trainFrontPos = _trainRoot.GlobalPosition;
            var desiredCamPos = trainFrontPos + _cameraOffset + (_trainRoot.GlobalTransform.Basis.X * _manualOffset);
            _camera.GlobalPosition = _camera.GlobalPosition.Lerp(desiredCamPos, (float)(CameraLerpSpeed * delta));

            _speedLabel.Text = mv.IsBraking
                ? $"▸ {mv.Speed:F0} u/s   {mv.DistanceTraveled / 1000f:F1} km   ▌▌ BRAKE"
                : $"▸ {mv.Speed:F0} u/s   {mv.DistanceTraveled / 1000f:F1} km";
        }
    }
}