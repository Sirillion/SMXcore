using UnityEngine;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.

//	Adds extra attributes to set the background sprites and color
//	Difference: Vanilla has no attributes for this and thus these attributes are hardcoded

namespace SMXcore
{
    public class XUiC_TraderItemEntry : global::XUiC_TraderItemEntry
    {
        private string selectedSpriteName = "ui_game_select_row";
        private string notSelectedSpriteName = "menu_empty";

        private Color selectedBackgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        private Color notSelectedBackgroundColor = new Color32(64, 64, 64, byte.MaxValue);

        public override void Update(float _dt)
        {
            base.Update(_dt);
            if (isDirty)
            {
                isDirty = false;
                ViewComponent.IsVisible = ViewComponent.Enabled;
            }
        }

        public override bool ParseAttribute(string name, string value, XUiController parent)
        {
            switch (name)
            {
                case "selectedbackgroundsprite":
                    selectedSpriteName = value;
                    return true;
                case "notselectedbackgroundsprite":
                    notSelectedSpriteName = value;
                    return true;
                case "selectedbackgroundcolor":
                    selectedBackgroundColor = StringParsers.ParseColor32(value);
                    return true;
                case "notselectedbackgroundcolor":
                    notSelectedBackgroundColor = StringParsers.ParseColor32(value);
                    return true;
                default:
                    return base.ParseAttribute(name, value, parent);
            }
        }

        public override void SelectedChanged(bool isSelected)
        {
            if (background != null)
            {
                background.Color = (isSelected ? selectedBackgroundColor : notSelectedBackgroundColor);
                background.SpriteName = (isSelected ? selectedSpriteName : notSelectedSpriteName);
            }
        }
    }
}
