using System;
using BepInEx.Configuration;
using Vapok.Common.Abstractions;
using Vapok.Common.Managers.Configuration;

namespace SaveYourLuckBruh.Configuration;

public class ConfigRegistry : ConfigSyncBase
{
    internal static ConfigEntry<bool> Enabled;

    public static Waiting Waiter;

    public ConfigRegistry(IPluginInfo mod, bool enableLockedConfigs = false) : base(mod, enableLockedConfigs)
    {
        Waiter = new Waiting();

        InitializeConfigurationSettings();
    }

    public sealed override void InitializeConfigurationSettings()
    {
        if (_config == null)
            return;

        UnsyncedConfig("Local Settings", "Enable SaveYourLuckBruh", true,
            new ConfigDescription("If enabled, enables SaveYourLuckBruh features.",
                null, new ConfigurationManagerAttributes { Category = "Local Settings", Order = 1 }), ref Enabled);

    }
}

public class Waiting
{
    public void ConfigurationComplete(bool configDone)
    {
        if (configDone)
            StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler StatusChanged;
}
