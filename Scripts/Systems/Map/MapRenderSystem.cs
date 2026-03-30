using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Map;

public class MapRenderSystem(Node3D environmentRoot) : ISystem
{
    private bool _isGenerated = false;

    public void Update(Core.ECS.World world, double delta)
    {
        if (_isGenerated) return;

        var mapEntity = world.Query<MapComponent>().FirstOrDefault();
        if (mapEntity == null) return;

        var map = world.Get<MapComponent>(mapEntity);
        DrawMap(map);

        _isGenerated = true;
    }

    private void DrawMap(MapComponent map)
    {
        var railMaterial = new StandardMaterial3D { AlbedoColor = new Color(0.8f, 0.4f, 0.1f) };
        var cityMaterial = new StandardMaterial3D { AlbedoColor = new Color(0.2f, 0.6f, 1.0f, 0.8f), Transparency = BaseMaterial3D.TransparencyEnum.Alpha };

        foreach (var node in map.AllNodes.Values)
        {
            var radius = 120f + GD.Randf() * 20f;
            var cityMesh = new MeshInstance3D { Name = $"Node_{node.Id}" };
            cityMesh.Mesh = new CylinderMesh { Height = 0.2f, TopRadius = radius, BottomRadius = radius };
            cityMesh.SetSurfaceOverrideMaterial(0, cityMaterial);

            var nodePos3D = new Vector3(node.Position.X, 0.1f, node.Position.Y);
            cityMesh.Position = nodePos3D;
            environmentRoot.AddChild(cityMesh);

            foreach (var nextPos3D in node.NextNodes.Select(nextId => map.AllNodes[nextId]).Select(nextNode => new Vector3(nextNode.Position.X, 0.1f, nextNode.Position.Y)))
                DrawRail(nodePos3D, nextPos3D, railMaterial);
        }
    }

    private void DrawRail(Vector3 start, Vector3 end, Material mat)
    {
        var distance = start.DistanceTo(end);
        var midPoint = (start + end) / 2f;

        var railMesh = new MeshInstance3D();
        railMesh.Mesh = new BoxMesh { Size = new Vector3(2f, 0.2f, distance) };
        railMesh.SetSurfaceOverrideMaterial(0, mat);

        railMesh.Position = midPoint;
        railMesh.LookAtFromPosition(midPoint, end, Vector3.Up);

        environmentRoot.AddChild(railMesh);
    }
}