using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuartzOverhaul;

namespace SMXcore
{
    public class XUiC_MapPopupList : XUiController
    {
        public override void Init()
        {
            base.Init();
            children[0].OnPress += onPressEntry1;
            children[1].OnPress += onPressEntry2;
        }

        private void onPressEntry1(XUiController _sender, int _mouseButton)
        {
            ((XUiC_MapArea)xui.GetWindow("mapArea").Controller).OnSetWaypoint();
        }

        private void onPressEntry2(XUiController _sender, int _mouseButton)
        {
            if (_sender is XUiC_MapPopupEntry)
            {
                XUiV_Window window = xui.GetWindow("mapAreaSetWaypoint");
                XUiV_Window window2 = xui.GetWindow("mapAreaChooseWaypoint");
                int num = 46;
                window2.Position = window.Position + new Vector2i(199, -num);
                if (window2.Position.y < 0)
                {
                    window2.Position = new Vector2i(window2.Position.x, window2.Position.y + window2.Size.y);
                }
                window2.IsVisible = true;
                window2.Controller.GetChildByType<XUiC_MapSubPopupList>().ResetList();
            }
        }
    }
}
