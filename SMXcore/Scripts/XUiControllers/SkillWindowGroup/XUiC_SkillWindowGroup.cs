
//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Changes the SkillWindowGroup XUiController to use the SMX SkillEntry and SkillSubEntry XUiControllers. Also replaces the CategoryList with SkillCategoryList

using GUI_2;

namespace SMXcore
{
    public class XUiC_SkillWindowGroup : XUiController
    {
        public XUiC_SkillEntry[] skillEntries;

        public XUiC_SkillList skillList;

        public XUiC_SkillCategoryList categoryList;

        public XUiC_SkillListWindow skillListWindow;
        public XUiC_SkillAttributeInfoWindow skillAttributeInfoWindow;
        public XUiC_SkillPerkInfoWindow skillPerkInfoWindow;
        public XUiC_SkillBookInfoWindow skillBookInfoWindow;
        public XUiC_SkillCraftingInfoWindow skillCraftingInfoWindow;

        public ProgressionValue currentSkill;

        public ProgressionValue CurrentSkill
        {
            get
            {
                return currentSkill;
            }
            set
            {
                currentSkill = value;
                IsDirty = true;
            }
        }

        public override void Init()
        {
            base.Init();
            skillList = GetChildByType<XUiC_SkillList>();

            skillListWindow = GetChildByType<XUiC_SkillListWindow>();

            skillAttributeInfoWindow = GetChildByType<XUiC_SkillAttributeInfoWindow>();
            skillPerkInfoWindow = GetChildByType<XUiC_SkillPerkInfoWindow>();
            skillBookInfoWindow = GetChildByType<XUiC_SkillBookInfoWindow>();
            skillCraftingInfoWindow = GetChildByType<XUiC_SkillCraftingInfoWindow>();

            skillEntries = GetChildrenByType<XUiC_SkillEntry>(null);
            for (int i = 0; i < skillEntries.Length; i++)
            {
                skillEntries[i].OnPress += XUiC_SkillEntry_OnPress;
            }
        }

        private void CategoryList_CategoryChanged(XUiC_SkillCategoryEntry categoryEntry)
        {
            IsDirty = true;
        }

        private void CategoryList_CategoryClickChanged(XUiC_SkillCategoryEntry categoryEntry)
        {
            IsDirty = true;
        }

        private void XUiC_SkillEntry_OnPress(XUiController _sender, int _mouseButton)
        {
            IsDirty = true;
        }

        public override void OnClose()
        {
            base.OnClose();
            xui.playerUI.windowManager.CloseIfOpen("windowpaging");
            xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
        }

        public override void Update(float _dt)
        {
            base.Update(_dt);
            if (IsDirty)
            {
                currentSkill = xui.selectedSkill;
                skillAttributeInfoWindow.SkillChanged();
                skillPerkInfoWindow.SkillChanged();
                skillBookInfoWindow.SkillChanged();
                skillCraftingInfoWindow.SkillChanged();
                if (skillListWindow.CategoryType == ProgressionClass.DisplayTypes.Book)
                {
                    skillBookInfoWindow.ViewComponent.IsVisible = true;
                }
                else if (skillListWindow.CategoryType == ProgressionClass.DisplayTypes.Crafting)
                {
                    skillCraftingInfoWindow.ViewComponent.IsVisible = true;
                }
                else if (xui.selectedSkill != null)
                {
                    if (xui.selectedSkill.ProgressionClass.IsAttribute)
                    {
                        skillAttributeInfoWindow.ViewComponent.IsVisible = true;
                    }
                    else
                    {
                        skillPerkInfoWindow.ViewComponent.IsVisible = true;
                    }
                }
                IsDirty = false;
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            if (categoryList == null)
            {
                XUiC_SkillCategoryList childByType = GetChildByType<XUiC_SkillCategoryList>();
                if (childByType != null)
                {
                    categoryList = childByType;
                    categoryList.SetupSkillCategories();
                    categoryList.CategoryChanged += CategoryList_CategoryChanged;
                    categoryList.CategoryClickChanged += CategoryList_CategoryClickChanged;
                }
            }
            xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
            xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
            xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
            xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.Menu);
            xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);

            XUiC_WindowSelector childByType2 = xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>();
            if (childByType2 != null)
            {
                childByType2.SetSelected("skills");
            }

            IsDirty = true;
            if (categoryList.CurrentCategory == null)
            {
                categoryList.SetCategoryToFirst();
            }
            skillListWindow.CategoryType = categoryList.CurrentCategory.CategoryType;
            skillList.RefreshSkillList();
            if (xui.selectedSkill == null)
            {
                skillList.SelectFirstEntry();
            }
            else
            {
                skillList.SelectedEntry.SelectCursorElement(true, false);
            }
            IsDirty = true;
        }

    }
}
