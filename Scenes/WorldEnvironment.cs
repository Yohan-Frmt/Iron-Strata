using Godot;

namespace IronStrata.Scenes;

public static class WorldEnvironment
{
    public static void Setup(Node root)
    {
        SetupWorldEnvironment(root);
        SetupAmbientLight(root);
    }

    private static void SetupWorldEnvironment(Node root)
    {
        var env = new Environment();

        env.BackgroundMode = Environment.BGMode.Color;
        env.BackgroundColor = new Color(0.005f, 0.005f, 0.01f);
        // env.BackgroundIntensity = 0f;

        env.AmbientLightSource = Environment.AmbientSource.Color;
        env.AmbientLightColor = new Color(0.06f, 0.07f, 0.10f);
        env.AmbientLightEnergy = 0.4f;

        env.SsaoEnabled = true;
        env.SsaoRadius = 1.2f;
        env.SsaoIntensity = 2.8f;
        env.SsaoDetail = 0.5f;

        env.FogEnabled = false;

        env.VolumetricFogEnabled = true;
        env.VolumetricFogDensity = 0.003f;
        env.VolumetricFogLength = 400f;
        env.VolumetricFogAlbedo = new Color(0.01f, 0.01f, 0.02f);
        env.VolumetricFogEmission = new Color(0, 0, 0);
        env.VolumetricFogAmbientInject = 0.8f;

        env.GlowEnabled = true;
        env.GlowIntensity = 1.0f;
        env.GlowStrength = 1.4f;
        env.GlowBloom = 0.1f;
        env.GlowHdrThreshold = 0.8f;

        env.TonemapMode = Environment.ToneMapper.Filmic;
        env.TonemapExposure = 1.1f;
        env.TonemapWhite = 6.0f;

        env.AdjustmentEnabled = true;
        env.AdjustmentBrightness = 0.88f;
        env.AdjustmentContrast = 1.25f;
        env.AdjustmentSaturation = 0.72f;

        root.AddChild(new Godot.WorldEnvironment { Name = "WorldEnvironment", Environment = env });
    }

    private static void SetupAmbientLight(Node root)
    {
        var dirLight = new DirectionalLight3D
        {
            LightColor = new Color(0.55f, 0.60f, 0.80f),
            LightEnergy = 2f,
            ShadowEnabled = true,
            Rotation = new Vector3(Mathf.DegToRad(-70f), Mathf.DegToRad(30f), 0f)
        };
        root.AddChild(dirLight);
    }
}