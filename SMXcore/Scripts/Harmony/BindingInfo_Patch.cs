using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore.HarmonyPatches
{
    [HarmonyPatch(typeof(BindingInfo))]
    public class BindingInfo_Harmony
    {

        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(XUiView), typeof(string), typeof(string) })]
        public static void Constructor(BindingInfo __instance, XUiView _view, string _property, string _sourceText)
        {
            //Refreshes the bindingInfo if its sourceText matches a set text.
            //This allows XUiControllers to set the size of the grid on creation so the right amount of rows or columns are created
            if (_sourceText.Contains("smxskilllistrows") || _sourceText.Contains("smxskillcategoryrows"))
            {
                __instance.RefreshValue(true);
            }
        }
    }
}
