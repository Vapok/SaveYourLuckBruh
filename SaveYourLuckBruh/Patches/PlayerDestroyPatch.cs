using HarmonyLib;
using SaveYourLuckBruh.Core;

namespace SaveYourLuckBruh.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.OnDestroy))]
internal static class PlayerDestroyPatch
{
    private static void Postfix(Player __instance)
    {
        if (__instance == Player.m_localPlayer)
        {
            StorageManager.Clear();
        }
    }
}
