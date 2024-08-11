using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using SMXcore.HarmonyPatches;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SMXcoreMod : IModApi, IGearsModApi
{
    public const string TAG = "SMXcore";
    public void InitMod(Mod _modInstance)
    {
        Log.Out($"[{TAG}] Loading Patch");
        var harmony = SMXHarmonyPatcher.GetHarmonyInstance();

        //Patching SMXlib
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Log.Out($"[{TAG}] Loaded Patch");
    }

    public void InitMod(IGearsMod modInstance)
    {
    }

    public void OnGlobalSettingsLoaded(IModGlobalSettings modSettings)
    {
        IGlobalModSettingsTab tab = modSettings.GetTab("General");

        IGlobalModSettingsCategory cat = tab.GetCategory("General");

        IGlobalValueSetting modSetting = cat.GetSetting("ForceSkipNews") as IGlobalValueSetting;
        SMXSettings.SkipNewsScreen(modSetting, modSetting.CurrentValue);
    }

    public void OnWorldSettingsLoaded(IModWorldSettings worldSettings)
    {
    }
}