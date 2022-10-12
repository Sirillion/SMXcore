using UnityEngine;
//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Sirillion, Laydor.

//	Changes the hard coded sprite and color values to fit SMX theme. Also Changes where the MapEnterWaypoint window opens
//	Difference: Vanilla has hard coded sprite and color values for the waypoint entries which clash with the SMX theme.
//	Difference: Vanilla opens up the MapEnterWaypoint window next to the waypoint icon selected, this opens up the window to below the mapAreaSetWaypoint window.

namespace SMXcore
{
    public class XUiC_MapSubPopupEntry : XUiController
    {
        private int index;
        private string spriteName;

        public override void Init()
        {
            base.Init();
            OnPress += onPressed;
            OnVisiblity += XUiC_MapSubPopupEntry_OnVisiblity;
        }

        private void XUiC_MapSubPopupEntry_OnVisiblity(XUiController _sender, bool _visible)
        {
            select(false);
        }

        public void SetIndex(int _idx)
        {
            index = _idx;
        }

        public void SetSpriteName(string _s)
        {
            spriteName = _s;
            for (int i = 0; i < Parent.Children.Count; i++)
            {
                XUiView viewComponent = Parent.Children[i].ViewComponent;
                if (viewComponent.ID.EqualsCaseInsensitive("icon"))
                {
                    ((XUiV_Sprite)viewComponent).SpriteName = _s;
                }
            }
        }

        protected override void OnHovered(bool _isOver)
        {
            select(_isOver);
        }

        private void onPressed(XUiController _sender, int _mouseButton)
        {
            select(true);
            ((XUiC_MapArea)xui.GetWindow("mapArea").Controller).OnWaypointEntryChosen(spriteName);
            XUiC_MapEnterWaypoint childByType = xui.GetWindow("mapAreaEnterWaypointName").Controller.GetChildByType<XUiC_MapEnterWaypoint>();
            XUiV_Window windowMapPopup = xui.GetWindow("mapAreaSetWaypoint");
            Vector2i position = windowMapPopup.Position + new Vector2i(0, -windowMapPopup.Size.y);
            childByType.Show(position);
        }

        private void select(bool _bSelect)
        {
            XUiV_Sprite xuiV_Sprite = viewComponent as XUiV_Sprite;
            if (xuiV_Sprite != null)
            {
                xuiV_Sprite.Color = (_bSelect ? new Color32(96, 96, 96, byte.MaxValue) : new Color32(64, 64, 64, byte.MaxValue));
                xuiV_Sprite.SpriteName = (_bSelect ? "smxui_button_background" : "smxui_button_background");
            }
        }

        public void Reset()
        {
            select(false);
        }
    }
}
