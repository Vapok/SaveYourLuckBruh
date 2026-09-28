using HarmonyLib;
using SaveYourLuckBruh.Configuration;
using SaveYourLuckBruh.Core;

namespace SaveYourLuckBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.Save))]
internal static class PlayerSavePatch
{
    private static void Prefix(Player __instance)
    {
        if (!ConfigRegistry.Enabled.Value)
        {
            return;
        }

        if (__instance == Player.m_localPlayer && ZNet.instance != null)
        {
            long worldUID = ZNet.instance.GetWorldUID();
            StorageManager.SaveToCustomData(__instance, worldUID);
        }
    }
}
