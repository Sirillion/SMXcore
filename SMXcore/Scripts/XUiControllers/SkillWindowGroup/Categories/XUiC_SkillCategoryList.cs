using GUI_2;
using Quartz;
using System.Collections.Generic;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Similar to CategoryList, but uses SkillCategoryEntry instead of CategoryEntry. Also determines how many skill categories there are

namespace SMXcore
{
    public class XUiC_SkillCategoryList : XUiController
    {
        private readonly List<XUiC_SkillCategoryEntry> categoryButtons = new List<XUiC_SkillCategoryEntry>();

        private int currentIndex;

        private XUiC_SkillCategoryEntry currentCategory;

        public bool AllowUnselect;

        public bool AllowKeyPaging = true;

        public XUiC_SkillCategoryEntry CurrentCategory
        {
            get
            {
                return currentCategory;
            }
            set
            {
                if (currentCategory != null)
                {
                    currentCategory.Selected = false;
                }

                currentCategory = value;
                if (currentCategory != null)
                {
                    currentCategory.Selected = true;
                    currentIndex = categoryButtons.IndexOf(currentCategory);
                }
            }
        }

        public int MaxCategories => categoryButtons.Count;

        public delegate void XUiEvent_SkillCategoryChangedEventHandler(XUiC_SkillCategoryEntry _categoryEntry);

        public event XUiEvent_SkillCategoryChangedEventHandler CategoryChanged;

        public event XUiEvent_SkillCategoryChangedEventHandler CategoryClickChanged;

        public override void Init()
        {
            base.Init();
            GetChildrenByType(categoryButtons);
            for (int i = 0; i < categoryButtons.Count; i++)
            {
                categoryButtons[i].CategoryList = this;
            }
        }

        public override void Update(float _dt)
        {
            base.Update(_dt);
            if (AllowKeyPaging && xui.playerUI.windowManager.IsKeyShortcutsAllowed())
            {
                PlayerActionsGUI gUIActions = xui.playerUI.playerInput.GUIActions;
                if (gUIActions.PageUp.WasReleased)
                {
                    IncrementCategory(1);
                }
                else if (gUIActions.PageDown.WasReleased)
                {
                    IncrementCategory(-1);
                }
            }
        }

        internal void HandleCategoryChanged()
        {
            CategoryChanged?.Invoke(CurrentCategory);
            CategoryClickChanged?.Invoke(CurrentCategory);
        }

        private XUiC_SkillCategoryEntry GetCategoryByName(string _category, out int _index)
        {
            _index = 0;
            for (int i = 0; i < categoryButtons.Count; i++)
            {
                if (categoryButtons[i].CategoryName == _category)
                {
                    _index = i;
                    return categoryButtons[i];
                }
            }

            return null;
        }

        public XUiC_SkillCategoryEntry GetCategoryByIndex(int _index)
        {
            if (_index >= categoryButtons.Count)
            {
                return null;
            }

            return categoryButtons[_index];
        }

        public void SetCategoryToFirst()
        {
            CurrentCategory = categoryButtons[0];
            CategoryChanged?.Invoke(CurrentCategory);
        }

        public void SetCategory(string _category)
        {
            int _index;
            XUiC_SkillCategoryEntry categoryByName = GetCategoryByName(_category, out _index);
            if (categoryByName != null || AllowUnselect)
            {
                CurrentCategory = categoryByName;
                CategoryChanged?.Invoke(CurrentCategory);
            }
        }

        private void IncrementCategory(int _offset)
        {
            if (_offset == 0)
            {
                return;
            }

            int i = 0;
            int num = NGUIMath.RepeatIndex(currentIndex + _offset, categoryButtons.Count);
            XUiC_SkillCategoryEntry xUiC_CategoryEntry = categoryButtons[num];
            for (; i < categoryButtons.Count; i++)
            {
                if (xUiC_CategoryEntry != null && !(xUiC_CategoryEntry.SpriteName == ""))
                {
                    break;
                }

                num = NGUIMath.RepeatIndex((_offset > 0) ? (num + 1) : (num - 1), categoryButtons.Count);
                xUiC_CategoryEntry = categoryButtons[num];
            }

            if (xUiC_CategoryEntry != null && xUiC_CategoryEntry.SpriteName != "")
            {
                CurrentCategory = xUiC_CategoryEntry;
                HandleCategoryChanged();
            }
        }

        public void SetCategoryEmpty(int _index)
        {
            XUiC_SkillCategoryEntry xUiC_CategoryEntry = categoryButtons[_index];
            xUiC_CategoryEntry.SpriteName = "";
            xUiC_CategoryEntry.CategoryDisplayName = "";
            xUiC_CategoryEntry.CategoryName = "";
            xUiC_CategoryEntry.ViewComponent.IsVisible = false;
        }

        public void SetCategoryEntry(int _index, string _categoryName, string _spriteName, string _displayName = null)
        {
            XUiC_SkillCategoryEntry xUiC_CategoryEntry = categoryButtons[_index];
            xUiC_CategoryEntry.CategoryDisplayName = _displayName ?? _categoryName;
            xUiC_CategoryEntry.CategoryName = _categoryName;
            xUiC_CategoryEntry.SpriteName = _spriteName ?? "";
            xUiC_CategoryEntry.ViewComponent.IsVisible = true;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
            xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftTrigger, "igcoCategoryLeft", XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
            xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightTrigger, "igcoCategoryRight", XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
            xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
        }

        public override void OnClose()
        {
            base.OnClose();
            xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "smxskillcategoryrows":
                    value = GetSkillCategoryRowCount().ToString();
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        public override bool ParseAttribute(string _name, string _value, XUiController _parent)
        {
            switch (_name)
            {
                case "allow_unselect":
                    AllowUnselect = StringParsers.ParseBool(_value);
                    return true;
                case "allow_key_paging":
                    AllowKeyPaging = StringParsers.ParseBool(_value);
                    return true;
                default:
                    return base.ParseAttribute(_name, _value, _parent);
            }
        }

        public bool SetupSkillCategories()
        {
            int index = 0;
            foreach (KeyValuePair<string, ProgressionClass> progressionClass in Progression.ProgressionClasses)
            {
                if (progressionClass.Value.IsAttribute)
                {
                    SetCategoryEntry(index, progressionClass.Value.Name, progressionClass.Value.Icon, Localization.Get(progressionClass.Value.Name));
                    index++;
                }
            }
            return true;
        }

        private int GetSkillCategoryRowCount()
        {
            int rowCount = 0;

            foreach (ProgressionClass progressionClass in Progression.ProgressionClasses.Values)
            {
                if (progressionClass.IsAttribute)
                {
                    rowCount++;
                }
            }

            Logging.Inform("SkillCategoryList", "Skill Category Row Count = " + rowCount);

            return rowCount;
        }
    }
}
