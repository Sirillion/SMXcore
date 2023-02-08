
//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Changes the SkillListWindow XUiController to use the SMX SkillEntry and SkillSubEntry XUiControllers. Also replaces the CategoryList with SkillCategoryList

namespace SMXcore
{
    public class XUiC_SkillListWindow : XUiController
    {
        private string totalItems = "";

        private int count;

        private string categoryName = "Intellect";

        private string categoryIcon = "";

        private string pointsAvailable;

        private string skillsTitle = "";

        private string booksTitle = "";

        private XUiC_SkillCategoryList categoryList;

        private XUiC_SkillList skillList;

        private readonly CachedStringFormatter<string, int> totalSkillsFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => string.Format(_s, _i));

        private readonly CachedStringFormatter<string, int> skillPointsAvailableFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => $"{_s} {_i}");

        public override void Init()
        {
            base.Init();
            totalItems = Localization.Get("lblTotalItems");
            pointsAvailable = Localization.Get("xuiPointsAvailable");
            skillsTitle = Localization.Get("xuiSkills");
            booksTitle = Localization.Get("lblCategoryBooks");
            skillList = GetChildByType<XUiC_SkillList>();
            XUiController childByType = GetChildByType<XUiC_SkillCategoryList>();
            if (childByType != null)
            {
                categoryList = (XUiC_SkillCategoryList)childByType;
                categoryList.CategoryChanged += CategoryList_CategoryChanged;
            }

            skillList.CategoryList = categoryList;
            skillList.SkillListWindow = this;
        }

        private void CategoryList_CategoryChanged(XUiC_SkillCategoryEntry _categoryEntry)
        {
            categoryName = _categoryEntry.CategoryDisplayName;
            categoryIcon = _categoryEntry.SpriteName;
            count = skillList.GetActiveCount();
            RefreshBindings();
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
                    if (skillList != null)
                    {
                        value = (skillList.IsBook ? booksTitle : skillsTitle);
                    }

                    return true;
                case "categoryicon":
                    value = categoryIcon;
                    return true;
                case "isnormal":
                    if (skillList != null)
                    {
                        value = (!skillList.IsBook).ToString();
                    }

                    return true;
                case "isbook":
                    if (skillList != null)
                    {
                        value = skillList.IsBook.ToString();
                    }

                    return true;
                case "skillpointsavailable":
                    {
                        string v = pointsAvailable;
                        EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
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
    }
}
