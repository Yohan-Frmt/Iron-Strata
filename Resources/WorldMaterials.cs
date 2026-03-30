using Godot;
using Godot.Collections;

namespace IronStrata.Resources;

public static class WorldMaterials
{
    private static readonly Dictionary<Color, StandardMaterial3D> Cache = new();

    public static StandardMaterial3D WagonMaterial(Color baseColor)
    {
        if (Cache.TryGetValue(baseColor, out var cached)) return cached;

        var mat = new StandardMaterial3D
        {
            AlbedoColor = baseColor,
            Metallic = 0.55f,
            MetallicSpecular = 0.3f,
            Roughness = 0.88f,
            AOEnabled = true,
            AOLightAffect = 0.6f,
        };

        Cache[baseColor] = mat;
        return mat;
    }

    public static readonly StandardMaterial3D Concrete = new()
    {
        AlbedoColor = new Color(0.12f, 0.12f, 0.14f),
        Metallic = 0.0f,
        Roughness = 0.97f,
        AOEnabled = true,
        AOLightAffect = 0.8f,
    };

    public static readonly StandardMaterial3D StructuralMetal = new()
    {
        AlbedoColor = new Color(0.09f, 0.09f, 0.11f),
        Metallic = 0.85f,
        Roughness = 0.65f,
    };

    public static readonly StandardMaterial3D EmissiveCold = new()
    {
        AlbedoColor = new Color(0.0f, 0.0f, 0.0f),
        EmissionEnabled = true,
        Emission = new Color(0.3f, 0.7f, 1.0f),
        EmissionEnergyMultiplier = 1.8f,
    };

    public static readonly StandardMaterial3D EmissiveWarm = new()
    {
        AlbedoColor = new Color(0.0f, 0.0f, 0.0f),
        EmissionEnabled = true,
        Emission = new Color(1.0f, 0.55f, 0.2f),
        EmissionEnergyMultiplier = 1.2f,
    };
}