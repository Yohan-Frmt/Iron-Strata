using Godot;

namespace IronStrata.Scenes;

public static class WorldEnvironment {
    public static void Setup(Node root) {
        SetupWorldEnvironment(root);
        SetupAmbientLight(root);
    }

    private static void SetupWorldEnvironment(Node root) {
        Environment env = new() {
            BackgroundMode = Environment.BGMode.Color,
            BackgroundColor = new Color(0.005f, 0.005f, 0.01f),

            AmbientLightSource = Environment.AmbientSource.Color,
            AmbientLightColor = new Color(0.06f, 0.07f, 0.10f),
            AmbientLightEnergy = 0.4f,

            SsaoEnabled = true,
            SsaoRadius = 1.2f,
            SsaoIntensity = 2.8f,
            SsaoDetail = 0.5f,

            FogEnabled = false,

            VolumetricFogEnabled = true,
            VolumetricFogDensity = 0.003f,
            VolumetricFogLength = 400f,
            VolumetricFogAlbedo = new Color(0.01f, 0.01f, 0.02f),
            VolumetricFogEmission = new Color(0, 0, 0),
            VolumetricFogAmbientInject = 0.8f,

            GlowEnabled = true,
            GlowIntensity = 1.0f,
            GlowStrength = 1.4f,
            GlowBloom = 0.1f,
            GlowHdrThreshold = 0.8f,

            TonemapMode = Environment.ToneMapper.Filmic,
            TonemapExposure = 1.1f,
            TonemapWhite = 6.0f,

            AdjustmentEnabled = true,
            AdjustmentBrightness = 0.88f,
            AdjustmentContrast = 1.25f,
            AdjustmentSaturation = 0.72f
        };

        root.AddChild(new Godot.WorldEnvironment { Name = "WorldEnvironment", Environment = env });
    }

    private static void SetupAmbientLight(Node root) {
        DirectionalLight3D dirLight = new() {
            LightColor = new Color(0.55f, 0.60f, 0.80f),
            LightEnergy = 2f,
            ShadowEnabled = true,
            Rotation = new Vector3(Mathf.DegToRad(-70f), Mathf.DegToRad(30f), 0f)
        };
        root.AddChild(dirLight);
    }
}
