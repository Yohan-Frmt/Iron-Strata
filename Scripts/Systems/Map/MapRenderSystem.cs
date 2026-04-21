using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Systems.Map;

/// <summary>
/// The MapRenderSystem is responsible for rendering the map within the game world.
/// It relies on the ECS (Entity Component System) world to retrieve relevant components
/// and update the map rendering state as required.
/// </summary>
public class MapRenderSystem(Node3D environmentRoot) : ISystem {
    /// <summary>
    /// Indicates whether the map has been generated and rendered in the 3D scene.
    /// This flag prevents redundant processing by ensuring map generation occurs only once.
    /// </summary>
    private bool _isGenerated;

    /// <summary>
    /// Updates the map rendering system by checking the world state. If the map has not been generated,
    /// it queries for the MapComponent in the world, and upon successful retrieval, draws the map and marks it as generated.
    /// </summary>
    /// <param name="world">The ECS world containing entities and components to interact with for querying and rendering.</param>
    /// <param name="delta">The time that has passed since the last update, used for frame-dependent behaviors if necessary.</param>
    public void Update(World world, double delta) {
        if (_isGenerated) { return; }

        foreach (MapComponent map in world.QueryWith<MapComponent>()) {
            DrawMap(map);
            _isGenerated = true;
            break;
        }
    }

    /// <summary>
    /// Draws the world map by creating visual representations of nodes and their connecting paths.
    /// Uses predefined materials for nodes and connections and places them within the 3D environment root.
    /// </summary>
    /// <param name="map">The map component containing the data structure of all nodes and their relationships.</param>
    private void DrawMap(MapComponent map) {
        StandardMaterial3D railMaterial = new() { AlbedoColor = new Color(0.8f, 0.4f, 0.1f) };
        StandardMaterial3D cityMaterial = new() {
            AlbedoColor = new Color(0.2f, 0.6f, 1.0f, 0.8f),
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha
        };

        foreach (MapNode node in map.AllNodes.Values) {
            float radius = node.Radius;
            MeshInstance3D cityMesh = new() {
                Name = $"Node_{node.Id}",
                Mesh = new CylinderMesh { Height = 0.2f, TopRadius = radius, BottomRadius = radius }
            };
            cityMesh.SetSurfaceOverrideMaterial(0, cityMaterial);

            Vector3 nodePosition3D = new(node.Position.X, 0.1f, node.Position.Y);
            cityMesh.Position = nodePosition3D;
            environmentRoot.AddChild(cityMesh);

            foreach (int nextNodeId in node.NextNodes) {
                MapNode nextNode = map.AllNodes[nextNodeId];
                Vector3 nextPosition3D = new(nextNode.Position.X, 0.1f, nextNode.Position.Y);
                DrawRail(nodePosition3D, nextPosition3D, railMaterial, node.Id, nextNode.Id);
            }
        }
    }

    /// <summary>
    /// Renders a rail segment between two points in 3D space using a series of interpolated box meshes.
    /// The rail follows a Bézier curve defined by the start and end points, as well as their associated IDs.
    /// </summary>
    /// <param name="start">The starting point of the rail segment in 3D space.</param>
    /// <param name="end">The ending point of the rail segment in 3D space.</param>
    /// <param name="material">The material to be applied to the rail mesh segments.</param>
    /// <param name="startId">The unique identifier of the starting node, used to calculate the Bézier curve.</param>
    /// <param name="endId">The unique identifier of the ending node, used to calculate the Bézier curve.</param>
    private void DrawRail(Vector3 start, Vector3 end, Material material, int startId, int endId) {
        const int segments = 250;
        Vector3 lastPoint = start;

        for (int segmentIndex = 1; segmentIndex <= segments; segmentIndex++) {
            float interpolationFactor = segmentIndex / (float)segments;
            Vector3 currentPoint = RailSampler.SampleBezier(start, end, startId, endId, interpolationFactor);
            float segmentDistance = lastPoint.DistanceTo(currentPoint);
            Vector3 midPoint = (lastPoint + currentPoint) / 2f;
            MeshInstance3D railMesh = new() { Mesh = new BoxMesh { Size = new Vector3(2f, 0.2f, segmentDistance + 0.1f) } };
            railMesh.SetSurfaceOverrideMaterial(0, material);
            railMesh.Position = midPoint;
            railMesh.LookAtFromPosition(midPoint, currentPoint, Vector3.Up);
            environmentRoot.AddChild(railMesh);
            lastPoint = currentPoint;
        }
    }
}
