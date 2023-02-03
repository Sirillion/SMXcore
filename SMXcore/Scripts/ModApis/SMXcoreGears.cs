using Gears.ModManager;
using Gears.ModManager.Settings;
using SMXcore.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class AMXcoreGears : IGearsModApi
    {
        public const string TAG = "SMXcore";

        public void InitMod(IGearsMod modInstance)
        {
            Log.Out($"[{TAG}] Loading Patch");
            var harmony = SMXHarmonyPatcher.GetHarmonyInstance();

            //Patching SMXlib
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Out($"[{TAG}] Loaded Patch");
        }

        public void OnDisabled()
        {
            
        }

        public void OnEnabled()
        {
            
        }

        public void OnSettingsLoaded(IModSettings modSettings)
        {
        }

        public void OnStart()
        {
            
        }

        public void OnStop()
        {
            
        }

        public bool RequireReset()
        {
            return false;
        }
    }
}
