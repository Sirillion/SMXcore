using System;
using System.Collections.Generic;
using System.ComponentModel;
//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.
//	Tweaked: .

//	This prevents the background sprite from being overwritten by the hardcoded values

namespace SMXcore
{
    public class XUiC_ItemActionEntry : global::XUiC_ItemActionEntry
    {

        private string spriteName;

        public override void Init()
        {
            base.Init();

            background.Controller.OnPress += OnPress;
            background.Controller.OnHover += OnHover;
        }

        public override void Update(float _dt)
        {
            spriteName = itemActionEntry != null ? itemActionEntry.IconName : "";
            base.Update(_dt);
            if(background.SpriteName != spriteName)
            {
                background.SpriteName = spriteName;
            }
        }

        private new void OnHover(XUiController sender, bool isOver)
        {
            XUiV_Sprite xuiV_Sprite = (XUiV_Sprite)sender.ViewComponent;
            xuiV_Sprite.SpriteName = spriteName;
        }

        private new void OnPress(XUiController sender, int mouseButton)
        {
            background.SpriteName = spriteName;
        }
    }
}
