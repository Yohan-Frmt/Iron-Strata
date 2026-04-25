using Godot;
using System.Collections.Generic;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Data;

namespace IronStrata.Scripts.Registry;

/// <summary>
/// Centralized registry for data-driven resources, such as enemies and wagons.
/// Scans defined directories for .tres files and populates internal dictionaries.
/// </summary>
public static class DataRegistry
{
    /// <summary>
    /// Dictionary mapping enemy types to their corresponding data resources.
    /// </summary>
    public static readonly Dictionary<EnemyType, EnemyData> EnemyDataMap = new();

    /// <summary>
    /// Dictionary mapping wagon types to their corresponding data resources.
    /// </summary>
    public static readonly Dictionary<WagonType, WagonData> WagonDataMap = new();

    /// <summary>
    /// Dictionary mapping card IDs (filenames) to their corresponding data resources.
    /// </summary>
    public static readonly Dictionary<string, CardData> CardDataMap = new();

    /// <summary>
    /// Initializes the registry by scanning for and loading all data resources.
    /// </summary>
    public static void Initialize()
    {
        EnemyDataMap.Clear();
        WagonDataMap.Clear();
        CardDataMap.Clear();

        LoadResources<EnemyData>("res://Resources/Data/Enemies", (data) => EnemyDataMap[data.Type] = data);
        LoadResources<WagonData>("res://Resources/Data/Wagons", (data) => WagonDataMap[data.Type] = data);
        LoadResources<CardData>("res://Resources/Data/Cards", (data) => {
            string id = data.ResourcePath.GetFile().GetBaseName();
            CardDataMap[id] = data;
        });

        GD.Print($"[DataRegistry] Loaded {EnemyDataMap.Count} enemies, {WagonDataMap.Count} wagons, and {CardDataMap.Count} cards.");
    }

    /// <summary>
    /// Scans a directory for .tres files of a specific resource type and loads them.
    /// </summary>
    /// <typeparam name="T">The type of resource to load.</typeparam>
    /// <param name="path">The path to the directory to scan.</param>
    /// <param name="onLoaded">Action to perform on each successfully loaded resource.</param>
    private static void LoadResources<T>(string path, System.Action<T> onLoaded) where T : Resource
    {
        if (!DirAccess.DirExistsAbsolute(path))
        {
            GD.PushWarning($"[DataRegistry] Directory does not exist: {path}");
            return;
        }

        using DirAccess dir = DirAccess.Open(path);
        if (dir == null)
        {
            GD.PushWarning($"[DataRegistry] Could not open directory: {path}");
            return;
        }

        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (fileName != "")
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".tres"))
            {
                T resource = GD.Load<T>($"{path}/{fileName}");
                if (resource != null)
                {
                    onLoaded(resource);
                }
                else
                {
                    GD.PushError($"[DataRegistry] Failed to load resource at: {path}/{fileName}");
                }
            }
            fileName = dir.GetNext();
        }
    }
}
