using Quartz;
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
            XUiC_PartList_Patch.PatchSetSlotMethod();
        }

        public new void SetSlot(ItemValue part, int index)
        {
            XUiC_ItemStack xUiC_ItemStack = itemControllers[index];
            xUiC_ItemStack.ViewComponent.IsVisible = true;
            if (part != null && !part.IsEmpty())
            {
                ItemStack itemStack = new ItemStack(part.Clone(), 1);
                xUiC_ItemStack.ItemStack = itemStack;
                xUiC_ItemStack.GreyedOut = false;
            }
            else
            {
                xUiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
                xUiC_ItemStack.GreyedOut = false;
            }

            itemControllers[index].ViewComponent.EventOnPress = false;
            itemControllers[index].ViewComponent.EventOnHover = false;
        }

        public new void SetSlots(ItemValue[] parts, int startIndex = 0)
        {
            if (startIndex == 0)
            {
                if (HasCosemticMods())
                {
                    SetSlot(mainItem.itemValue.CosmeticMods[0], 0);
                }
                else
                {
                    itemControllers[0].ItemStack = ItemStack.Empty.Clone();
                    itemControllers[0].ViewComponent.IsVisible = false;

                }
                    startIndex = 1;
            }

            for (int i = 0; i < itemControllers.Length - startIndex; i++)
            {
                int num = i + startIndex;
                XUiC_ItemStack xUiC_ItemStack = itemControllers[num];
                if(parts.Length > i)
                {
                    xUiC_ItemStack.ViewComponent.IsVisible = true;
                    if (parts[i] != null && !parts[i].IsEmpty())
                    {
                        ItemStack itemStack = new ItemStack(parts[i].Clone(), 1);
                        xUiC_ItemStack.ItemStack = itemStack;
                        xUiC_ItemStack.GreyedOut = false;
                        xUiC_ItemStack.ViewComponent.IsVisible = true;
                    }
                    else
                    {
                        xUiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
                        xUiC_ItemStack.GreyedOut = false;
                    }
                } 
                else
                {
                    xUiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
                    xUiC_ItemStack.GreyedOut = false;
                    xUiC_ItemStack.ViewComponent.IsVisible = false;
                }

                xUiC_ItemStack.ViewComponent.EventOnPress = false;
                xUiC_ItemStack.ViewComponent.EventOnHover = false;
            }
        }

        public new void SetMainItem(ItemStack itemStack)
        {
            mainItem = itemStack;
        }

        public bool HasCosemticMods()
        {
            return mainItem.itemValue.CosmeticMods != null && mainItem.itemValue.CosmeticMods.Length > 0 && mainItem.itemValue.CosmeticMods[0] != null;
        }
    }
}
