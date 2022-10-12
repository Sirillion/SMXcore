using GUI_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_SkillWindowGroup : XUiController
    {

        private XUiC_SkillList skillList;

        private XUiC_CategoryList categoryList;

        private XUiC_SkillAttributeInfoWindow skillAttributeInfoWindow;

        private XUiC_SkillSkillInfoWindow skillSkillInfoWindow;

        private XUiC_SkillPerkInfoWindow skillPerkInfoWindow;

        private XUiC_SkillBookInfoWindow skillBookInfoWindow;

        private ProgressionValue currentSkill;

        public ProgressionValue CurrentSkill
        {
            get
            {
                return currentSkill;
            }
            set
            {
                currentSkill = value;
            }
        }

        public override void Init()
        {
            base.Init();
            skillList = GetChildByType<XUiC_SkillList>();
            skillAttributeInfoWindow = GetChildByType<XUiC_SkillAttributeInfoWindow>();
            skillSkillInfoWindow = GetChildByType<XUiC_SkillSkillInfoWindow>();
            skillPerkInfoWindow = GetChildByType<XUiC_SkillPerkInfoWindow>();
            skillBookInfoWindow = GetChildByType<XUiC_SkillBookInfoWindow>();
            XUiC_SkillEntry[] skillEntries = GetChildrenByType<XUiC_SkillEntry>();
            foreach(XUiC_SkillEntry entry in skillEntries)
            {
                entry.OnPress += XUiC_SkillEntry_OnPress;
            }

            categoryList = GetChildByType<XUiC_CategoryList>();
            if (categoryList != null)
            {
                categoryList.SetupCategoriesByWorkstation("skills");
                categoryList.CategoryChanged += CategoryList_CategoryChanged;
                categoryList.CategoryClickChanged += CategoryList_CategoryClickChanged;
            }
        }

        private void CategoryList_CategoryChanged(XUiC_CategoryEntry _categoryEntry)
        {
            skillList.Category = _categoryEntry.CategoryName;
            skillList.RefreshSkillList();
            skillList.SelectFirstEntry();
            IsDirty = true;
        }

        private void CategoryList_CategoryClickChanged(XUiC_CategoryEntry _categoryEntry)
        {
            skillList.Category = _categoryEntry.CategoryName;
            skillList.SetFilterText("");
            skillList.RefreshSkillList();
            skillList.SelectFirstEntry();
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
        }

        public override void Update(float _dt)
        {
            base.Update(_dt);
            if (!IsDirty)
            {
                return;
            }

            CurrentSkill = base.xui.selectedSkill;
            skillAttributeInfoWindow.SkillChanged();
            skillSkillInfoWindow.IsDirty = true;
            skillPerkInfoWindow.SkillChanged();
            skillBookInfoWindow.SkillChanged();
            if (skillList.IsBook)
            {
                skillBookInfoWindow.ViewComponent.IsVisible = true;
            }
            else if (xui.selectedSkill != null)
            {
                if (xui.selectedSkill.ProgressionClass.IsAttribute)
                {
                    skillAttributeInfoWindow.ViewComponent.IsVisible = true;
                }
                else if (xui.selectedSkill.ProgressionClass.IsSkill)
                {
                    skillSkillInfoWindow.ViewComponent.IsVisible = true;
                }
                else
                {
                    skillPerkInfoWindow.ViewComponent.IsVisible = true;
                }
            }

            IsDirty = false;
        }

        public override void OnOpen()
        {
            base.OnOpen();

            xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", _bModal: false);
            xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>()?.SetSelected("skills");
            IsDirty = true;
            if (categoryList.CurrentCategory == null)
            {
                categoryList.SetCategoryToFirst();
            }

            skillList.Category = categoryList.CurrentCategory.CategoryName;
            skillList.RefreshSkillList();
            skillList.SelectFirstEntry();
            IsDirty = true;
        }
    }
}
