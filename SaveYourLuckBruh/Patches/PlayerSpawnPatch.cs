using HarmonyLib;
using SaveYourLuckBruh.Configuration;
using SaveYourLuckBruh.Core;

namespace SaveYourLuckBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
internal static class PlayerSpawnPatch
{
    private static void Postfix(Player __instance)
    {
        if (!ConfigRegistry.Enabled.Value)
        {
            return;
        }

        if (__instance == Player.m_localPlayer && ZNet.instance != null && ZNet.m_world != null)
        {
            long worldUID = ZNet.instance.GetWorldUID();
            StorageManager.EnsureLoaded(__instance, worldUID);
        }
    }
}
