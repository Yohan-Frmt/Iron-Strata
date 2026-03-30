using Godot;

namespace IronStrata.Scenes;

public static class WorldEnvironment
{
    public static void Setup(Node root)
    {
        SetupWorldEnvironment(root);
        SetupAmbientLight(root);
        SetupTunnelLights(root);
    }

    private static void SetupWorldEnvironment(Node root)
    {
        var env = new Environment();

        env.BackgroundMode = Environment.BGMode.Color;
        env.BackgroundColor = new Color(0.01f, 0.01f, 0.015f);
        env.BackgroundIntensity = 0f;

        env.AmbientLightSource = Environment.AmbientSource.Color;
        env.AmbientLightColor = new Color(0.06f, 0.07f, 0.10f);
        env.AmbientLightEnergy = 0.15f;

        env.SsaoEnabled = true;
        env.SsaoRadius = 1.2f;
        env.SsaoIntensity = 2.8f;
        env.SsaoDetail = 0.5f;

        env.FogEnabled = true;
        env.FogMode = Environment.FogModeEnum.Depth;
        env.FogLightColor = new Color(0.06f, 0.08f, 0.12f);
        env.FogDensity = 0.018f;
        env.FogDepthBegin = 18f;
        env.FogDepthEnd = 55f;

        env.GlowEnabled = true;
        env.GlowIntensity = 0.7f;
        env.GlowStrength = 1.4f;
        env.GlowBloom = 0.15f;
        env.GlowHdrThreshold = 0.8f;

        env.TonemapMode = Environment.ToneMapper.Filmic;
        env.TonemapExposure = 1.1f;
        env.TonemapWhite = 6.0f;

        env.AdjustmentEnabled = true;
        env.AdjustmentBrightness = 0.88f;
        env.AdjustmentContrast = 1.25f;
        env.AdjustmentSaturation = 0.72f;

        root.AddChild(new Godot.WorldEnvironment { Environment = env });
    }

    private static void SetupAmbientLight(Node root)
    {
        var dirLight = new DirectionalLight3D
        {
            LightColor = new Color(0.55f, 0.60f, 0.80f),
            LightEnergy = 0.12f,
            ShadowEnabled = true,
            Rotation = new Vector3(Mathf.DegToRad(-70f), Mathf.DegToRad(30f), 0f)
        };
        root.AddChild(dirLight);
    }

    private static void SetupTunnelLights(Node root)
    {
        var positions = new (Vector3 pos, Color color, float energy, float range)[]
        {
            (new Vector3(5f, 6f, 0f), new Color(0.4f, 0.7f, 1.0f), 1.5f, 12f),
            (new Vector3(22f, 6f, 0f), new Color(0.3f, 0.6f, 0.9f), 1.2f, 10f),
            (new Vector3(38f, 6f, 0f), new Color(0.4f, 0.7f, 1.0f), 1.5f, 12f),
            (new Vector3(12f, -1f, 0f), new Color(1.0f, 0.5f, 0.2f), 0.8f, 7f),
        };

        foreach (var (pos, color, energy, range) in positions)
        {
            root.AddChild(new OmniLight3D
            {
                Position = pos,
                LightColor = color,
                LightEnergy = energy,
                OmniRange = range,
                ShadowEnabled = true
            });
        }
    }
}