using Godot;

namespace IronStrata.Scripts.Core.Constants;

/// <summary>
/// Contains constants and utility methods for defining the layout of a train.
/// </summary>
/// <remarks>
/// This class provides dimensions for train wagons and gaps, color definitions for different wagon types,
/// and a method for calculating the local position of a wagon slot within a train.
/// </remarks>
public static class TrainLayout {
    /// <summary>
    /// Represents the longitudinal dimension of a single train wagon in the layout.
    /// This value is used for positioning wagons along the train, defining their size,
    /// and calculating spacing between components in the system.
    /// </summary>
    public const float WagonLength = 4.0f;

    /// <summary>
    /// Represents the horizontal dimension of a single train wagon in the layout.
    /// This value determines the width used for rendering visual components and
    /// calculating collision shapes, ensuring accurate representation and
    /// interaction in the system.
    /// </summary>
    public const float WagonWidth = 6.0f;

    /// <summary>
    /// Represents the vertical dimension of a single train wagon in the layout.
    /// This value defines the height used for rendering and collision calculations
    /// in the system, ensuring visual and functional consistency across wagons.
    /// </summary>
    public const float WagonHeight = 3.2f;

    /// <summary>
    /// Specifies the horizontal gap or spacing between adjacent train wagons in the layout configuration.
    /// This value influences the overall length of the train and the spatial separation between wagons.
    /// </summary>
    public const float WagonGap = 0.3f;

    /// <summary>
    /// Defines the vertical offset applied between different layers in the train layout.
    /// This value is used to calculate the relative positioning of elements on separate layers
    /// within the layout, ensuring proper spacing along the vertical axis.
    /// </summary>
    public const float LayerOffset = 3.4f;


    /// <summary>
    /// Represents the default color applied to the locomotive wagons in the train layout.
    /// This color is used for visual differentiation of locomotive entities in the system.
    /// </summary>
    public static readonly Color ColorLoco = new(0.18f, 0.18f, 0.22f);

    /// <summary>
    /// Represents the color associated with combat wagons in the train layout.
    /// This color is used for visual differentiation of combat-related components
    /// and ensures a consistent visual theme for combat entities throughout the system.
    /// </summary>
    public static readonly Color ColorCombat = new(0.30f, 0.08f, 0.08f);

    /// <summary>
    /// Defines the color associated with living quarters in the train layout.
    /// This value is primarily used to provide a visual distinction for train wagons
    /// designated for habitation, enhancing clarity and consistency in the system's rendering
    /// and representation.
    /// </summary>
    public static readonly Color ColorLiving = new(0.16f, 0.12f, 0.08f);

    /// <summary>
    /// Represents the designated color used for storage wagons within the train system.
    /// This color is applied to visual components to distinguish storage wagons from other types,
    /// ensuring clarity in both appearance and functionality across the system.
    /// </summary>
    public static readonly Color ColorStorage = new(0.08f, 0.14f, 0.22f);

    /// <summary>
    /// Defines the color associated with research wagons in the train layout.
    /// This color is used for rendering and visual identification of research wagons
    /// within the system.
    /// </summary>
    public static readonly Color ColorResearch = new(0.14f, 0.08f, 0.22f);

    /// <summary>
    /// Specifies the color used to visually represent medical facilities within the train layout.
    /// This color helps to identify wagons designated for medical purposes, ensuring clarity
    /// in visual organization and wagon categorization.
    /// </summary>
    public static readonly Color ColorMedical = new(0.06f, 0.18f, 0.12f);

    /// <summary>
    /// Calculates the local position of a wagon slot based on its slot index and layer.
    /// </summary>
    /// <param name="slotIndex">The index of the wagon slot. This determines the horizontal offset.</param>
    /// <param name="layer">The layer of the wagon slot. This determines the vertical offset.</param>
    /// <returns>A <c>Vector3</c> representing the local position of the specified wagon slot.</returns>
    public static Vector3 GetLocalPosition(int slotIndex, int layer) {
        float x = -slotIndex * (WagonLength + WagonGap);
        float y = layer * LayerOffset;
        return new Vector3(x, y, 0f);
    }
}
