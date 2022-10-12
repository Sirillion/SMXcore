using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SMXcore.HarmonyPatches
{
    public static class XUiC_MapWaypointListEntry_Patch
    {
        public static void PatchUpdateSelectedMethod()
        {
            Harmony harmony = SMXHarmonyPatcher.GetHarmonyInstance();

            MethodInfo original = AccessTools.Method(typeof(XUiC_MapWaypointListEntry), "updateSelected");
            MethodInfo prefix = AccessTools.Method(typeof(XUiC_MapWaypointListEntry_Patch), "updateSelected");

            bool alreadyPatched = false;
            IEnumerable<MethodBase> OriginalMethods = harmony.GetPatchedMethods();
            foreach (MethodBase method in OriginalMethods)
            {
                if (method.Equals(original))
                {
                    alreadyPatched = true;
                    break;
                }
            }

            if (!alreadyPatched)
            {
                harmony.Patch(original, prefix: new HarmonyMethod(prefix));
            }
        }

        //Harmony Prefix Patch for "updateSelected"
        public static bool updateSelected(XUiC_MapWaypointListEntry __instance, ref bool _bHover, ref XUiV_Sprite ___Background, ref bool ___m_bSelected)
        {
            XUiV_Sprite background = ___Background;
            if (background != null)
            {
                if (___m_bSelected)
                {
                    background.Color = new Color32(160, 160, 160, byte.MaxValue);
                    background.SpriteName = "smxlib_window_button_background";
                }
                else if (_bHover)
                {
                    background.Color = new Color32(96, 96, 96, byte.MaxValue);
                    background.SpriteName = "smxlib_window_button_background";
                }
                else
                {
                    background.Color = new Color32(7, 7, 7, byte.MaxValue);
                    background.SpriteName = "smxlib_window_button_background";
                }
            }
            __instance.Tracking.IsVisible = (__instance.Waypoint != null && __instance.Waypoint.bTracked);
            return false;
        }
    }
}
