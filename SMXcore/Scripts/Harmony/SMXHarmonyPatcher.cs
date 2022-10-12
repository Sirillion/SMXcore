using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore.HarmonyPatches
{
    public static class SMXHarmonyPatcher
    {
        public const string Id = "Harmony.SMXcore.Mod";

        private static HarmonyLib.Harmony harmony;

        public static HarmonyLib.Harmony GetHarmonyInstance()
        {
            if(harmony == null)
            {
                harmony = new HarmonyLib.Harmony(Id);
            }

            return harmony;
        }
    }
}
