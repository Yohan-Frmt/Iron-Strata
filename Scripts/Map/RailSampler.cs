using Godot;

namespace IronStrata.Scripts.Map;

/// <summary>
/// Utility class for sampling rail positions and tangents.
/// Ensures consistent and varied rail curves across different systems.
/// </summary>
public static class RailSampler
{
    /// <summary>
    /// Calculates a point on a Bezier curve based on the given start and end points, control points derived
    /// from deterministic logic, and the interpolation parameter t.
    /// </summary>
    /// <param name="start">The starting point of the Bezier curve.</param>
    /// <param name="end">The ending point of the Bezier curve.</param>
    /// <param name="startId">An identifier for the starting node, used for deterministic calculations.</param>
    /// <param name="endId">An identifier for the ending node, used for deterministic calculations.</param>
    /// <param name="t">A parameter within the range [0, 1] used to determine the interpolated position on the curve.</param>
    /// <returns>A <see cref="Vector3"/> representing the interpolated point on the Bezier curve.</returns>
    public static Vector3 SampleBezier(Vector3 start, Vector3 end, int startId, int endId, float t)
    {
        // Deterministic hash based on node IDs
        var seed = (uint)(startId * 1337 ^ endId * 7331);
        
        var xDist = Mathf.Abs(end.X - start.X);
        var dist = start.DistanceTo(end);
        var controlOffset = xDist * 0.45f;
        var offsetVar = GetHash(seed, 1, 0.8f, 1.2f);
        var sidewaysVar1 = GetHash(seed, 2, -0.05f, 0.05f) * dist;
        var sidewaysVar2 = GetHash(seed, 3, -0.05f, 0.05f) * dist;
        var forwardVar1 = GetHash(seed, 4, -0.05f, 0.05f) * xDist;
        var forwardVar2 = GetHash(seed, 5, -0.05f, 0.05f) * xDist;
        var p1 = start + new Vector3(controlOffset * offsetVar + forwardVar1, 0, sidewaysVar1);
        var p2 = end - new Vector3(controlOffset * offsetVar + forwardVar2, 0, sidewaysVar2);
        var a = start.Lerp(p1, t);
        var b = p1.Lerp(p2, t);
        var c = p2.Lerp(end, t);
        var d = a.Lerp(b, t);
        var e = b.Lerp(c, t);
        return d.Lerp(e, t);
    }

    /// <summary>
    /// Generates a deterministic hash value based on the given seed and index, and maps it to a specified range.
    /// </summary>
    /// <param name="seed">The seed used for hash generation, typically derived from unique identifiers.</param>
    /// <param name="index">An additional parameter for varying hash values, ensuring uniqueness for different indices.</param>
    /// <param name="min">The minimum value of the desired range.</param>
    /// <param name="max">The maximum value of the desired range.</param>
    /// <returns>A floating-point value within the specified range [min, max], derived from the hash computation.</returns>
    private static float GetHash(uint seed, int index, float min, float max)
    {
        var h = seed ^ ((uint)index * 0x85ebca6b);
        h ^= h >> 13;
        h *= 0xc2b2ae35;
        h ^= h >> 16;
        var normalized = (h & 0x7FFFFFFF) / (float)0x7FFFFFFF;
        return min + normalized * (max - min);
    }
}
