using HarmonyLib;
using SaveYourLuckBruh.Configuration;
using SaveYourLuckBruh.Core;

namespace SaveYourLuckBruh.Patches;

internal class FejdStartupPatches
{
    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Awake))]
    [HarmonyAfter("vapok.common.LocalizationManager", "org.bepinex.helpers.LocalizationManager")]
    [HarmonyBefore("vapok.common.ItemManager", "org.bepinex.helpers.ItemManager")]
    internal static class FejdStartupAwakePatch
    {
        private static void Prefix()
        {
            SaveYourLuckBruh.Waiter.ValheimIsAwake(true);
            CharacterDrop.s_pseudoCounter.Clear();
            StorageManager.Clear();
        }
    }

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.OnNewCharacterDone))]
    internal static class FejdStartupNewCharacterDonePatch
    {
        private static void Prefix()
        {
            if (!ConfigRegistry.Enabled.Value)
            {
                return;
            }

            StorageManager.Clear();
        }
    }
}
