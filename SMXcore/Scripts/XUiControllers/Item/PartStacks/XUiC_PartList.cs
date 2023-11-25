using SMXcore.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.
//	Tweaked: .

//	Reserves the item modification parts list slot 0 to a cosmetic mod whether it exists or not
//	Difference: If there is no cosmetic mod, all the remaining installed mods will shift to slot 1

namespace SMXcore
{
    public class XUiC_PartList : global::XUiC_PartList
    {
        public ItemStack mainItem;

        public override void Init()
        {
            base.Init();
            XUiC_PartList_Patch.PatchSetMainItemMethod();
            XUiC_PartList_Patch.PatchSetSlotsMethod();
        }

        public new void SetSlots(ItemValue[] parts, int startIndex = 0)
        {
            if (startIndex == 0 && HasCosemticMods())
            {
                SetSlot(mainItem.itemValue.CosmeticMods[0], 0);
                startIndex = 1;
            }

            for (int i = 0; i < itemControllers.Length - startIndex; i++)
            {
                int num = i + startIndex;
                if (parts.Length > i && parts[i] != null && !parts[i].IsEmpty())
                {
                    ItemStack itemStack = new ItemStack(parts[i].Clone(), 1);
                    itemControllers[num].ItemStack = itemStack;
                    itemControllers[num].GreyedOut = false;
                }
                else
                {
                    itemControllers[num].ItemStack = ItemStack.Empty.Clone();
                    itemControllers[num].GreyedOut = false;
                }

                itemControllers[num].ViewComponent.EventOnPress = false;
                itemControllers[num].ViewComponent.EventOnHover = false;
            }
        }

        public void SetMainItem(ItemStack itemStack)
        {
            mainItem = itemStack;
        }

        public bool HasCosemticMods()
        {
            return mainItem.itemValue.CosmeticMods != null && mainItem.itemValue.CosmeticMods.Length > 0 && mainItem.itemValue.CosmeticMods[0] != null;
        }
    }
}
