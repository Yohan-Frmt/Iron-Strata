using Godot;
namespace IronStrata.Scripts.Core.Constants;

public static class TrainLayout
{
    public const float WagonLength  = 4.0f;
    public const float WagonWidth   = 6.0f;
    public const float WagonHeight  = 3.2f;
    public const float WagonGap     = 0.3f;
    public const float LayerOffset  = 3.4f;

    public static readonly Color ColorLoco     = new(0.18f, 0.18f, 0.22f);
    public static readonly Color ColorCombat   = new(0.30f, 0.08f, 0.08f);
    public static readonly Color ColorLiving   = new(0.16f, 0.12f, 0.08f);
    public static readonly Color ColorStorage  = new(0.08f, 0.14f, 0.22f);
    public static readonly Color ColorResearch = new(0.14f, 0.08f, 0.22f);
    public static readonly Color ColorMedical  = new(0.06f, 0.18f, 0.12f);
    
    public static Vector3 GetLocalPosition(int slotIndex, int layer)
    {
        var x = -slotIndex * (WagonLength + WagonGap);
        var y = layer * LayerOffset;
        return new Vector3(x, y, 0f);
    }
}