using HarmonyLib;
using UnityEngine;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: TormentedEmu.
//	Tweaked: Sirillion, Laydor.

//	Adds an extra binding to hide buff background depending on a buff existing or not.
//	Difference: Vanilla has no binding for this, and as such every grid cell would get a background drawn regardless of it having a buff or not.

namespace SMXcore
{
    public class XUiC_ActiveBuffEntry : global::XUiC_ActiveBuffEntry
    {
        private XUiV_Sprite background;

        public override void Init()
        {
            base.Init();
            background = GetChildById("background").ViewComponent as XUiV_Sprite;
        }
        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "hasbuff":
                    value = (Notification != null && Notification.Buff != null) ? "true" : "false";
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        protected override void SelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                InfoWindow.SetBuffInfo(this);
            }

            if (background != null)
            {
                // this controls the color of the background sprite
                background.Color = (isSelected ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) :   // Sprite color when selected
                                                    new Color32(96, 96, 96, byte.MaxValue));                                    // Sprite color when not selected

                // this controls which sprite will be shown when the buff is selected(or clicked on) in the active buff list(Character window)
                background.SpriteName = (isSelected ? "smxlib_slot_frame_narrow" :                                           // Sprite to use when buff currently selected
                                                         "smxlib_slot_frame_narrow");                                           // Sprite to use when buff not selected
            }
        }
    }
}

