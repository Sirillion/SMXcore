using SMXcore.HarmonyPatches;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SMXcoreMod : IModApi
{
    public const string TAG = "SMXcore";
    public void InitMod(Mod _modInstance)
    {
        Log.Out($"[{TAG}] Loading Patch");
        var harmony = SMXHarmonyPatcher.GetHarmonyInstance();

        //Patching SMXlib
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Log.Out($"[{TAG}] Loaded Patch");

        //Log.Out($"[{TAG}] Checking Dependencies");
        //SMXDependencyChecker.CheckOutdatedDependencies();
        //SMXDependencyChecker.CheckMissingDependencies();
    }
}