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

        Dictionary<string, Tuple<float, int>> initialCounters = new Dictionary<string, Tuple<float, int>>(CharacterDrop.s_pseudoCounter);
        __state = new DropEvaluationState(attacker, initialCounters);
    }

    private static void Postfix(CharacterDrop __instance, List<KeyValuePair<GameObject, int>> __result, DropEvaluationState __state)
    {
        if (!ConfigRegistry.Enabled.Value || __instance == null || __state == null)
        {
            return;
        }

        Character attacker = __state.Attacker;
        bool isLocalPlayer = attacker != null && attacker == Player.m_localPlayer;
        bool isPlayerPet = attacker != null && attacker.IsTamed();
        bool isOtherPlayer = attacker != null && attacker.IsPlayer() && attacker != Player.m_localPlayer;

        string attackerLabel = isLocalPlayer ? "LocalPlayer" : (isPlayerPet ? "Pet" : (isOtherPlayer ? $"Ally ({attacker.GetHoverName()})" : "Environmental"));

        foreach (CharacterDrop.Drop drop in __instance.m_drops)
        {
            if (drop == null || drop.m_prefab == null)
            {
                continue;
            }

            string prefabName = drop.m_prefab.name;
            if (CharacterDrop.s_pseudoCounter.TryGetValue(prefabName, out Tuple<float, int> currentValue))
            {
                bool hadInitial = __state.InitialCounters.TryGetValue(prefabName, out Tuple<float, int> initialValue);
                int prevCount = hadInitial ? initialValue.Item2 : -1;
                int currentCount = currentValue.Item2;
                float chance = currentValue.Item1;

                bool didDrop = false;
                if (__result != null)
                {
                    for (int i = 0; i < __result.Count; i++)
                    {
                        if (__result[i].Key == drop.m_prefab)
                        {
                            didDrop = true;
                            break;
                        }
                    }
                }

                if (hadInitial && prevCount <= 1 && currentCount > 1 && didDrop)
                {
                    SaveYourLuckBruh.Log.Debug($"Guaranteed bad luck drop triggered for {prefabName}! Next threshold rolled: {currentCount.ToString(CultureInfo.InvariantCulture)}.");
                }
                else
                {
                    SaveYourLuckBruh.Log.Debug($"Bad luck drop evaluated for {prefabName} (chance: {chance.ToString("P1", CultureInfo.InvariantCulture)}). Counter: {prevCount.ToString(CultureInfo.InvariantCulture)} -> {currentCount.ToString(CultureInfo.InvariantCulture)}. Dropped: {didDrop.ToString()}. Attacker: {attackerLabel}.");
                }
            }
        }

        if (Player.m_localPlayer != null && ZNet.instance != null && ZNet.m_world != null)
        {
            long worldUID = ZNet.instance.GetWorldUID();
            StorageManager.UpdateMemoryCache(Player.m_localPlayer, worldUID);
        }
    }
}
