using Quartz;
using SMXcore.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_MapInvitesListEntry : Quartz.XUiC_MapInvitesListEntry
    {

        public override void Init()
        {
            base.Init();
            XUiC_MapInvitesListEntry_Patch.PatchUpdateSelectedMethod();
        }
    }
}
