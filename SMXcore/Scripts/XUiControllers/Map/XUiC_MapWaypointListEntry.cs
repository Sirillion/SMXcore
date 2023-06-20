using Quartz;
using SMXcore.HarmonyPatches;

namespace SMXcore
{
    public class XUiC_MapWaypointListEntry : Quartz.XUiC_MapWaypointListEntry
    {
        public override void Init()
        {
            base.Init();
            XUiC_MapWaypointListEntry_Patch.PatchUpdateSelectedMethod();
        }
    }
}
