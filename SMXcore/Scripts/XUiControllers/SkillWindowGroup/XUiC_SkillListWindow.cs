
//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Changes the SkillListWindow XUiController to use the SMX SkillEntry and SkillSubEntry XUiControllers. Also replaces the CategoryList with SkillCategoryList

using GUI_2;

namespace SMXcore
{
    public class XUiC_SkillListWindow : XUiController
    {
        private string totalItems = "";

        private string categoryName = "Intellect";
        private string categoryIcon = "";
        private string pointsAvailable;

        private string skillsTitle = "";
        private string booksTitle = "";
        private string craftingTitle = "";

        private XUiC_SkillCategoryList categoryList;
        private XUiC_SkillList skillList;
        private XUiC_BookList bookList;
        private XUiC_CraftingSkillList craftingSkillList;

        private readonly CachedStringFormatter<string, int> totalSkillsFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => string.Format(_s, _i));
        private readonly CachedStringFormatter<string, int> skillPointsAvailableFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => $"{_s} {_i}");

        public ProgressionClass.DisplayTypes CategoryType { get; set;}

        public override void Init()
        {
            base.Init();

            totalItems = Localization.Get("lblTotalItems");
            pointsAvailable = Localization.Get("xuiPointsAvailable");
            skillsTitle = Localization.Get("xuiSkills");
            booksTitle = Localization.Get("lblCategoryBooks");
            craftingTitle = Localization.Get("xuiCrafting");

            skillList = GetChildByType<XUiC_SkillList>();
            bookList = GetChildByType<XUiC_BookList>();
            craftingSkillList = GetChildByType<XUiC_CraftingSkillList>();

            XUiController childByType = GetChildByType<XUiC_SkillCategoryList>();
            if (childByType != null)
            {
                categoryList = (XUiC_SkillCategoryList)childByType;
                categoryList.CategoryChanged += CategoryList_CategoryChanged;
            }

            skillList.SkillListWindow = this;
            bookList.SkillListWindow = this;
            craftingSkillList.SkillListWindow = this;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
            xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, "igcoSpendPoints", XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
            xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts, 0f);
        }
        public override void OnClose()
        {
            base.OnClose();
            xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
        }

        private void CategoryList_CategoryChanged(XUiC_SkillCategoryEntry _categoryEntry)
        {
            CategoryType = _categoryEntry.CategoryType;
            categoryName = _categoryEntry.CategoryDisplayName;
            categoryIcon = _categoryEntry.SpriteName;
            RefreshBindings();
            SelectFirstEntry();
        }

        public void SetSelectedByUnlockData(RecipeUnlockData unlockData)
        {
            switch (unlockData.UnlockType)
            {
                case RecipeUnlockData.UnlockTypes.Perk:
                    //selectName = unlockData.Perk.Name;
                    if (unlockData.Perk.IsPerk)
                    {
                        categoryList.SetCategory(ProgressionClass.DisplayTypes.Standard);
                    }

                    break;
                case RecipeUnlockData.UnlockTypes.Book:
                    //selectName = unlockData.Perk.ParentName;
                    if (unlockData.Perk.IsPerk)
                    {
                        categoryList.SetCategory(ProgressionClass.DisplayTypes.Book);
                    }

                    break;
                case RecipeUnlockData.UnlockTypes.Skill:
                    //selectName = unlockData.Perk.Name;
                    if (unlockData.Perk.IsCrafting)
                    {
                        categoryList.SetCategory(ProgressionClass.DisplayTypes.Crafting);
                    }
                    break;
                default:
                    return;
            }
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "totalskills":
                    value = "";
                    if (skillList != null)
                    {
                        value = totalSkillsFormatter.Format(totalItems, skillList.GetActiveCount());
                    }

                    return true;
                case "titlename":
                    value = "";
                    switch (CategoryType)
                    {
                        case ProgressionClass.DisplayTypes.Standard:
                            value = skillsTitle;
                            break;
                        case ProgressionClass.DisplayTypes.Book:
                            value = booksTitle;
                            break;
                        case ProgressionClass.DisplayTypes.Crafting:
                            value = craftingTitle;
                            break;
                    }

                    return true;
                case "categoryicon":
                    value = categoryIcon;
                    return true;
                case "isnormal":
                    value = CategoryType == ProgressionClass.DisplayTypes.Standard ? "true" : "false";
                    return true;
                case "isbook":
                    value = CategoryType == ProgressionClass.DisplayTypes.Book ? "true" : "false";
                    return true;
                case "iscrafting":
                    value = CategoryType == ProgressionClass.DisplayTypes.Crafting ? "true" : "false";
                    return true;
                case "skillpointsavailable":
                    {
                        string v = pointsAvailable;
                        EntityPlayerLocal entityPlayer = xui.playerUI.entityPlayer;
                        if (XUi.IsGameRunning() && entityPlayer != null)
                        {
                            value = skillPointsAvailableFormatter.Format(v, entityPlayer.Progression.SkillPoints);
                        }

                        return true;
                    }
                default:
                    return false;
            }
        }

        public void SelectFirstEntry()
        {
            switch (CategoryType)
            {
                case ProgressionClass.DisplayTypes.Standard:
                    skillList.SelectFirstEntry();
                    break;
                case ProgressionClass.DisplayTypes.Book:
                    bookList.SelectFirstEntry();
                    break;
                case ProgressionClass.DisplayTypes.Crafting:
                    craftingSkillList.SelectFirstEntry();
                    break;
            }
        }

        public XUiC_SkillEntry GetEntryForSkill(ProgressionValue skill)
        {
            if(skill == null) return null;

            switch (skill.ProgressionClass.DisplayType)
            {
                case ProgressionClass.DisplayTypes.Standard:
                    return skillList.GetEntryForSkill(skill);
                case ProgressionClass.DisplayTypes.Book:
                    return bookList.GetEntryForSkill(skill);
                case ProgressionClass.DisplayTypes.Crafting:
                    return craftingSkillList.GetEntryForSkill(skill);
                default:
                    return null;
            }
        }
    }
}
