using System;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;
using SaveYourLuckBruh.Configuration;
using SaveYourLuckBruh.Core;
using UnityEngine;

namespace SaveYourLuckBruh.Patches;

[HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
internal static class CharacterDropPatch
{
    private static void Prefix(CharacterDrop __instance, out DropEvaluationState __state)
    {
        if (!ConfigRegistry.Enabled.Value || __instance == null)
        {
            __state = null;
            return;
        }

        Character character = __instance.GetComponent<Character>();
        Character attacker = null;

        if (character != null && character.m_lastHit != null)
        {
            attacker = character.m_lastHit.GetAttacker();
        }

        Dictionary<int, int> initialCounters = new Dictionary<int, int>(CharacterDrop.s_pseudoCounter);

        int level = character != null ? character.GetLevel() : 1;
        int levelMultiplier = Mathf.Max(1, (int)Mathf.Pow(2f, level - 1));

        for (int i = 0; i < __instance.m_drops.Count; i++)
        {
            CharacterDrop.Drop drop = __instance.m_drops[i];
            if (drop == null || drop.m_prefab == null)
            {
                continue;
            }

            float baseChance = drop.m_chance;
            if (baseChance <= 0f || baseChance > 0.3f || drop.m_amountMin != 1 || drop.m_amountMax != 1)
            {
                continue;
            }

            int baseHash = drop.m_prefab.name.GetStableHashCode() + (int)(baseChance * 1000f);

            if (!CharacterDrop.s_pseudoCounter.TryGetValue(baseHash, out int baseRemaining))
            {
                int maxRange = (int)(1f / baseChance * 2f);
                baseRemaining = UnityEngine.Random.Range(0, maxRange);
                CharacterDrop.s_pseudoCounter[baseHash] = baseRemaining;
            }

            float scaledChance = drop.m_levelMultiplier ? (baseChance * levelMultiplier) : baseChance;
            int starHash = drop.m_prefab.name.GetStableHashCode() + (int)(scaledChance * 1000f);

            if (starHash != baseHash && scaledChance <= 0.3f)
            {
                int starRemaining = Mathf.Max(0, (baseRemaining + 1) / 2);
                CharacterDrop.s_pseudoCounter[starHash] = starRemaining;
            }
        }

        __state = new DropEvaluationState(attacker, initialCounters);
    }

    private static void Postfix(CharacterDrop __instance, List<KeyValuePair<GameObject, int>> __result, DropEvaluationState __state)
    {
        if (!ConfigRegistry.Enabled.Value || __instance == null || __state == null)
        {
            return;
        }

        Character character = __instance.GetComponent<Character>();
        Character attacker = __state.Attacker;
        bool isLocalPlayer = attacker != null && attacker == Player.m_localPlayer;
        bool isPlayerPet = attacker != null && attacker.IsTamed();
        bool isOtherPlayer = attacker != null && attacker.IsPlayer() && attacker != Player.m_localPlayer;

        string attackerLabel = isLocalPlayer ? "LocalPlayer" : (isPlayerPet ? "Pet" : (isOtherPlayer ? $"Ally ({attacker.GetHoverName()})" : "Environmental"));

        int level = character != null ? character.GetLevel() : 1;
        int levelMultiplier = Mathf.Max(1, (int)Mathf.Pow(2f, level - 1));

        for (int i = 0; i < __instance.m_drops.Count; i++)
        {
            CharacterDrop.Drop drop = __instance.m_drops[i];
            if (drop == null || drop.m_prefab == null)
            {
                continue;
            }

            float baseChance = drop.m_chance;
            if (baseChance <= 0f || baseChance > 0.3f || drop.m_amountMin != 1 || drop.m_amountMax != 1)
            {
                continue;
            }

            string prefabName = drop.m_prefab.name;
            int baseHash = prefabName.GetStableHashCode() + (int)(baseChance * 1000f);

            float scaledChance = drop.m_levelMultiplier ? (baseChance * levelMultiplier) : baseChance;
            int starHash = prefabName.GetStableHashCode() + (int)(scaledChance * 1000f);

            bool didDrop = false;
            if (__result != null)
            {
                for (int j = 0; j < __result.Count; j++)
                {
                    if (__result[j].Key == drop.m_prefab)
                    {
                        didDrop = true;
                        break;
                    }
                }
            }

            __state.InitialCounters.TryGetValue(baseHash, out int initialBaseCount);

            if (starHash != baseHash && scaledChance <= 0.3f)
            {
                CharacterDrop.s_pseudoCounter.Remove(starHash);

                if (didDrop)
                {
                    int nextThreshold = UnityEngine.Random.Range(0, (int)(1f / baseChance * 2f) + 2);
                    CharacterDrop.s_pseudoCounter[baseHash] = nextThreshold;
                    SaveYourLuckBruh.Log.Debug($"Trophy drop success on {level.ToString(CultureInfo.InvariantCulture)}-star {prefabName}! Reset unified pity countdown to {nextThreshold.ToString(CultureInfo.InvariantCulture)}. Attacker: {attackerLabel}.");
                }
                else
                {
                    int currentBaseCount = Mathf.Max(1, initialBaseCount - levelMultiplier);
                    CharacterDrop.s_pseudoCounter[baseHash] = currentBaseCount;
                    SaveYourLuckBruh.Log.Debug($"Pity progress on {level.ToString(CultureInfo.InvariantCulture)}-star {prefabName} (weight {levelMultiplier.ToString(CultureInfo.InvariantCulture)}): {initialBaseCount.ToString(CultureInfo.InvariantCulture)} -> {currentBaseCount.ToString(CultureInfo.InvariantCulture)}. Attacker: {attackerLabel}.");
                }
            }
            else if (scaledChance > 0.3f)
            {
                if (didDrop)
                {
                    int nextThreshold = UnityEngine.Random.Range(0, (int)(1f / baseChance * 2f) + 2);
                    CharacterDrop.s_pseudoCounter[baseHash] = nextThreshold;
                    SaveYourLuckBruh.Log.Debug($"Trophy drop success on high-tier {level.ToString(CultureInfo.InvariantCulture)}-star {prefabName}! Reset unified pity countdown to {nextThreshold.ToString(CultureInfo.InvariantCulture)}. Attacker: {attackerLabel}.");
                }
                else
                {
                    int newCount = initialBaseCount - levelMultiplier;
                    if (newCount <= 0)
                    {
                        if (__result != null)
                        {
                            __result.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, 1));
                            didDrop = true;
                        }
                        int nextThreshold = UnityEngine.Random.Range(0, (int)(1f / baseChance * 2f) + 2);
                        CharacterDrop.s_pseudoCounter[baseHash] = nextThreshold;
                        SaveYourLuckBruh.Log.Debug($"Guaranteed bad luck drop triggered on {level.ToString(CultureInfo.InvariantCulture)}-star {prefabName}! Next unified threshold: {nextThreshold.ToString(CultureInfo.InvariantCulture)}. Attacker: {attackerLabel}.");
                    }
                    else
                    {
                        CharacterDrop.s_pseudoCounter[baseHash] = newCount;
                        SaveYourLuckBruh.Log.Debug($"Pity progress on {level.ToString(CultureInfo.InvariantCulture)}-star {prefabName} (weight {levelMultiplier.ToString(CultureInfo.InvariantCulture)}): {initialBaseCount.ToString(CultureInfo.InvariantCulture)} -> {newCount.ToString(CultureInfo.InvariantCulture)}. Attacker: {attackerLabel}.");
                    }
                }
            }
            else
            {
                if (CharacterDrop.s_pseudoCounter.TryGetValue(baseHash, out int currentCount))
                {
                    if (initialBaseCount <= 1 && currentCount > 1 && didDrop)
                    {
                        SaveYourLuckBruh.Log.Debug($"Guaranteed bad luck drop triggered for {prefabName}! Next threshold rolled: {currentCount.ToString(CultureInfo.InvariantCulture)}.");
                    }
                    else
                    {
                        SaveYourLuckBruh.Log.Debug($"Bad luck drop evaluated for {prefabName} (chance: {baseChance.ToString("P1", CultureInfo.InvariantCulture)}). Counter: {initialBaseCount.ToString(CultureInfo.InvariantCulture)} -> {currentCount.ToString(CultureInfo.InvariantCulture)}. Dropped: {didDrop.ToString()}. Attacker: {attackerLabel}.");
                    }
                }
            }

            if (!isLocalPlayer && !isPlayerPet && isOtherPlayer)
            {
                CharacterDrop.s_pseudoCounter[baseHash] = initialBaseCount;
                SaveYourLuckBruh.Log.Debug($"Preserved local player pity counter for {prefabName} ({initialBaseCount.ToString(CultureInfo.InvariantCulture)}) because kill was made by {attackerLabel}.");
            }
        }

        if (Player.m_localPlayer != null && ZNet.instance != null && ZNet.m_world != null)
        {
            long worldUID = ZNet.instance.GetWorldUID();
            StorageManager.UpdateMemoryCache(Player.m_localPlayer, worldUID);
        }
    }
}
