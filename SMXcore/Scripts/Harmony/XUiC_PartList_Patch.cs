using HarmonyLib;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore.HarmonyPatches
{
    public static class XUiC_PartList_Patch
    {

        public static void PatchSetMainItemMethod()
        {
            Harmony harmony = SMXHarmonyPatcher.GetHarmonyInstance();

            MethodInfo original = AccessTools.Method(typeof(global::XUiC_PartList), "SetMainItem");
            MethodInfo postfix = AccessTools.Method(typeof(XUiC_PartList_Patch), "SetMainItem");

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
                harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            }
        }

        public static void PatchSetSlotsMethod()
        {
            Harmony harmony = SMXHarmonyPatcher.GetHarmonyInstance();

            MethodInfo original = AccessTools.Method(typeof(global::XUiC_PartList), "SetSlots");
            MethodInfo prefix = AccessTools.Method(typeof(XUiC_PartList_Patch), "SetSlots");

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

        public static void SetMainItem(global::XUiC_PartList __instance, ItemStack itemStack)
        {
            if(__instance is XUiC_PartList partList)
            {
                partList.SetMainItem(itemStack);
            }
        }

        public static bool SetSlots(global::XUiC_PartList __instance, ItemValue[] parts, int startIndex)
        {
            if (__instance is XUiC_PartList partList)
            {
                partList.SetSlots(parts, startIndex);
                return false;
            }

            return true;
        }
    }
}
