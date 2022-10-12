using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.
//	Tweaked: .

//	Changes hard coded sprites to be SMX sprites.
//	Difference: Vanilla has these sprites hardcoded so when the action happens the sprite would shift to something else. This prevents that.
namespace SMXcore
{
    public class XUiC_ItemPartStack : global::XUiC_ItemPartStack
    {
        protected override void SelectedChanged(bool isSelected)
        {
            SetColor(isSelected ? selectColor : XUiC_BasePartStack.backgroundColor);
            ((XUiV_Sprite)background.ViewComponent).SpriteName = (isSelected ? "smxlib_slot_frame_narrow" : "smxlib_slot_frame_narrow");
        }
    }
}
