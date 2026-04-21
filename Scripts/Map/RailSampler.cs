using Godot;

namespace IronStrata.Scripts.Map;

/// <summary>
/// Utility class for sampling rail positions and tangents.
/// Ensures consistent and varied rail curves across different systems.
/// </summary>
public static class RailSampler {
    /// <summary>
    /// Calculates a point on a Bézier curve based on the given start and end points, control points derived
    /// from deterministic logic, and the interpolation parameter t.
    /// </summary>
    /// <param name="start">The starting point of the Bézier curve.</param>
    /// <param name="end">The ending point of the Bézier curve.</param>
    /// <param name="startId">An identifier for the starting node, used for deterministic calculations.</param>
    /// <param name="endId">An identifier for the ending node, used for deterministic calculations.</param>
    /// <param name="interpolationFactor">A parameter within the range [0, 1] used to determine the interpolated position on the curve.</param>
    /// <returns>A <see cref="Vector3"/> representing the interpolated point on the Bézier curve.</returns>
    public static Vector3 SampleBezier(Vector3 start, Vector3 end, int startId, int endId, float interpolationFactor) {
        uint seed = (uint)(startId * 1337 ^ endId * 7331);
        float xDistance = Mathf.Abs(end.X - start.X);
        float segmentDistance = start.DistanceTo(end);
        float controlOffset = xDistance * 0.45f;
        float offsetVar = GetHash(seed, 1, 0.8f, 1.2f);
        float sidewaysVar1 = GetHash(seed, 2, -0.05f, 0.05f) * segmentDistance;
        float sidewaysVar2 = GetHash(seed, 3, -0.05f, 0.05f) * segmentDistance;
        float forwardVar1 = GetHash(seed, 4, -0.05f, 0.05f) * xDistance;
        float forwardVar2 = GetHash(seed, 5, -0.05f, 0.05f) * xDistance;
        Vector3 controlPoint1 = start + new Vector3(controlOffset * offsetVar + forwardVar1, 0, sidewaysVar1);
        Vector3 controlPoint2 = end - new Vector3(controlOffset * offsetVar + forwardVar2, 0, sidewaysVar2);
        Vector3 interpolationA = start.Lerp(controlPoint1, interpolationFactor);
        Vector3 interpolationB = controlPoint1.Lerp(controlPoint2, interpolationFactor);
        Vector3 interpolationC = controlPoint2.Lerp(end, interpolationFactor);
        Vector3 interpolationD = interpolationA.Lerp(interpolationB, interpolationFactor);
        Vector3 interpolationE = interpolationB.Lerp(interpolationC, interpolationFactor);
        return interpolationD.Lerp(interpolationE, interpolationFactor);
    }

    /// <summary>
    /// Generates a deterministic hash value based on the given seed and index and maps it to a specified range.
    /// </summary>
    /// <param name="seed">The seed used for hash generation, typically derived from unique identifiers.</param>
    /// <param name="index">An additional parameter for varying hash values, ensuring uniqueness for different indices.</param>
    /// <param name="min">The minimum value of the desired range.</param>
    /// <param name="max">The maximum value of the desired range.</param>
    /// <returns>A floating-point value within the specified range [min, max], derived from the hash computation.</returns>
    private static float GetHash(uint seed, int index, float min, float max) {
        uint hashValue = seed ^ (uint)index * 0x85ebca6b;
        hashValue ^= hashValue >> 13;
        hashValue *= 0xc2b2ae35;
        hashValue ^= hashValue >> 16;
        float normalized = (hashValue & 0x7FFFFFFF) / (float)0x7FFFFFFF;
        return min + normalized * (max - min);
    }
}
