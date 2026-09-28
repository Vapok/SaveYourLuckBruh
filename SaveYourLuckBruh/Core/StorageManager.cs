using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SaveYourLuckBruh.Core;

internal static class StorageManager
{
    private const char EntryDelimiter = '|';
    private const char FieldDelimiter = ':';
    private const string ModPrefix = "vapok.mods.SaveYourLuckBruh";

    private static bool _isLoaded;

    public static bool IsLoaded => _isLoaded;

    public static string GetStorageKey(long worldUID)
    {
        return $"{ModPrefix}.{worldUID.ToString(CultureInfo.InvariantCulture)}";
    }

    public static void SaveToCustomData(Player player, long worldUID)
    {
        if (player == null || player.m_customData == null)
        {
            return;
        }

        string storageKey = GetStorageKey(worldUID);

        if (CharacterDrop.s_pseudoCounter.Count == 0)
        {
            if (player.m_customData.Remove(storageKey))
            {
                SaveYourLuckBruh.Log.Debug($"Cleared empty bad luck data from Player.m_customData for world {worldUID.ToString(CultureInfo.InvariantCulture)}.");
            }
            return;
        }

        StringBuilder stringBuilder = new StringBuilder();
        bool isFirst = true;

        foreach (KeyValuePair<string, Tuple<float, int>> entry in CharacterDrop.s_pseudoCounter)
        {
            if (!isFirst)
            {
                stringBuilder.Append(EntryDelimiter);
            }

            string prefabName = entry.Key;
            float chance = entry.Value.Item1;
            int counter = entry.Value.Item2;

            stringBuilder.Append(prefabName);
            stringBuilder.Append(FieldDelimiter);
            stringBuilder.Append(chance.ToString("R", CultureInfo.InvariantCulture));
            stringBuilder.Append(FieldDelimiter);
            stringBuilder.Append(counter.ToString(CultureInfo.InvariantCulture));

            isFirst = false;
        }

        string serializedData = stringBuilder.ToString();
        player.m_customData[storageKey] = serializedData;

        SaveYourLuckBruh.Log.Debug($"Saved {CharacterDrop.s_pseudoCounter.Count.ToString(CultureInfo.InvariantCulture)} bad luck counter(s) for world {worldUID.ToString(CultureInfo.InvariantCulture)}: {serializedData}");
    }

    public static void LoadFromCustomData(Player player, long worldUID)
    {
        if (player == null || player.m_customData == null)
        {
            return;
        }

        string storageKey = GetStorageKey(worldUID);
        CharacterDrop.s_pseudoCounter.Clear();
        _isLoaded = true;

        if (!player.m_customData.TryGetValue(storageKey, out string serializedData) || string.IsNullOrEmpty(serializedData))
        {
            SaveYourLuckBruh.Log.Debug($"No persistent bad luck counters found in Player.m_customData for world {worldUID.ToString(CultureInfo.InvariantCulture)}; starting fresh.");
            return;
        }

        string[] entries = serializedData.Split(EntryDelimiter);
        int loadedCount = 0;

        for (int i = 0; i < entries.Length; i++)
        {
            string entry = entries[i];
            if (string.IsNullOrEmpty(entry))
            {
                continue;
            }

            string[] fields = entry.Split(FieldDelimiter);
            if (fields.Length != 3)
            {
                SaveYourLuckBruh.Log.Warning($"Skipping malformed bad luck entry '{entry}' for world {worldUID.ToString(CultureInfo.InvariantCulture)}.");
                continue;
            }

            string prefabName = fields[0];
            if (float.TryParse(fields[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float chance) &&
                int.TryParse(fields[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int counter))
            {
                CharacterDrop.s_pseudoCounter[prefabName] = new Tuple<float, int>(chance, counter);
                loadedCount++;
                SaveYourLuckBruh.Log.Debug($"Restored bad luck counter for {prefabName} -> chance: {chance.ToString("P1", CultureInfo.InvariantCulture)}, remaining: {counter.ToString(CultureInfo.InvariantCulture)}.");
            }
            else
            {
                SaveYourLuckBruh.Log.Warning($"Failed to parse values for bad luck entry '{entry}'.");
            }
        }

        SaveYourLuckBruh.Log.Debug($"Successfully loaded {loadedCount.ToString(CultureInfo.InvariantCulture)} bad luck counter(s) for world {worldUID.ToString(CultureInfo.InvariantCulture)}.");
    }

    public static void EnsureLoaded(Player player, long worldUID)
    {
        if (!_isLoaded)
        {
            LoadFromCustomData(player, worldUID);
        }
    }

    public static void UpdateMemoryCache(Player player, long worldUID)
    {
        SaveToCustomData(player, worldUID);
    }

    public static void Clear()
    {
        CharacterDrop.s_pseudoCounter.Clear();
        _isLoaded = false;
        SaveYourLuckBruh.Log.Debug("Cleared in-memory bad luck counters.");
    }
}
