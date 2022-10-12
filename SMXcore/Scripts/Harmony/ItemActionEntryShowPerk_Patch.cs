using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore.HarmonyPatches
{
    public static class ItemActionEntryShowPerk_Patch
    {

        public static void PatchOnActivatedMethod()
        {
            Harmony harmony = SMXHarmonyPatcher.GetHarmonyInstance();

            MethodInfo original = AccessTools.Method(typeof(ItemActionEntryShowPerk), "OnActivated");
            MethodInfo prefix = AccessTools.Method(typeof(ItemActionEntryShowPerk_Patch), "OnActivated");

            bool alreadyPatched = false;
            IEnumerable<MethodBase> OriginalMethods = harmony.GetPatchedMethods();
            foreach (MethodBase method in OriginalMethods) 
            { 
                if(method.Equals(original))
                {
                    alreadyPatched = true;
                    break;
                }
            }

            if(!alreadyPatched)
            {
                harmony.Patch(original, prefix: new HarmonyMethod(prefix));
            }
        }

        //Harmony Prefix Patch for "OnActivated"
        public static bool OnActivated(ItemActionEntryShowPerk __instance)
        {
            XUi xui = __instance.ItemController.xui;
            List<XUiC_SkillList> childrenByType = xui.GetChildrenByType<XUiC_SkillList>();

            if(childrenByType.Count <= 0)
            {
                return true;
            }

            xui.playerUI.windowManager.CloseIfOpen("looting");

            XUiC_SkillList xuiC_SkillList = null;
            foreach (XUiC_SkillList SkillList in childrenByType)
            {
                if (SkillList.WindowGroup != null && SkillList.WindowGroup.isShowing)
                {
                    xuiC_SkillList = SkillList;
                    break;
                }
            }
            if (xuiC_SkillList == null)
            {
                XUiC_WindowSelector.OpenSelectorAndWindow(xui.playerUI.entityPlayer, "skills");
                xuiC_SkillList = xui.GetChildByType<XUiC_SkillList>();
            }

            if (xuiC_SkillList != null)
            {
                xuiC_SkillList.SetSelectedByUnlockData(__instance.UnlockData);
            }

            return false;
        }
        
    }
}
