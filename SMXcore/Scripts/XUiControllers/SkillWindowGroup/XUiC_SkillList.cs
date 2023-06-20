using SMXcore.HarmonyPatches;
using System.Collections.Generic;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Changes the SkillWindowGroup XUiController to use the SMX SkillEntry and SkillSubEntry XUiControllers. Also replaces the CategoryList with SkillCategoryList

namespace SMXcore
{
    public class XUiC_SkillList : XUiController
    {
        private List<ProgressionValue> skills = new List<ProgressionValue>();

        private List<ProgressionValue> currentSkills = new List<ProgressionValue>();

        private XUiC_SkillEntry[] skillEntries;

        private string filterText = "";

        private string selectName;

        private XUiC_SkillSubEntry selectedEntry;

        private XUiC_TextInput txtInput;

        private string category = "";

        public XUiC_SkillSubEntry SelectedEntry
        {
            get
            {
                return selectedEntry;
            }
            set
            {
                if (selectedEntry != null)
                {
                    selectedEntry.IsSelected = false;
                }

                selectedEntry = value;

                if (selectedEntry != null)
                {
                    selectedEntry.IsSelected = true;
                    xui.selectedSkill = selectedEntry.Skill;
                }
            }
        }

        public XUiC_SkillCategoryList CategoryList { get; set; }

        public XUiC_SkillListWindow SkillListWindow { get; set; }

        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                if (category != value)
                {
                    category = value;
                }
            }
        }

        public bool IsBook => Category == "attbooks";

        public override void Init()
        {
            base.Init();

            ItemActionEntryShowPerk_Patch.PatchOnActivatedMethod();

            Category = "";
            XUiController xUiController = parent.Parent;

            txtInput = xUiController.GetChildByType<XUiC_TextInput>();
            if (txtInput != null)
            {
                txtInput.OnChangeHandler += TxtInput_OnChangeHandler;
                txtInput.Text = "";
            }

            XUiController grid = GetChildById("skills");
            skillEntries = grid?.GetChildrenByType<XUiC_SkillEntry>();

            XUiC_SkillSubEntry[] subEntries = grid?.GetChildrenByType<XUiC_SkillSubEntry>();
            foreach (XUiC_SkillSubEntry entry in subEntries)
            {
                entry.OnPress += XUiC_SkillEntry_OnPress;
            }

        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "smxskilllistrows":
                    value = GetSkillListRowCount().ToString();
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        public void SetSelectedByUnlockData(RecipeUnlockData unlockData)
        {
            switch (unlockData.UnlockType)
            {
                case RecipeUnlockData.UnlockTypes.Perk:
                    selectName = unlockData.Perk.Name;
                    if (unlockData.Perk.IsPerk)
                    {
                        CategoryList.SetCategory(unlockData.Perk.Parent.ParentName);
                    }

                    break;
                case RecipeUnlockData.UnlockTypes.Book:
                    selectName = unlockData.Perk.ParentName;
                    if (unlockData.Perk.IsPerk)
                    {
                        CategoryList.SetCategory(unlockData.Perk.Parent.ParentName);
                    }

                    break;
            }
        }

        internal int GetActiveCount()
        {
            return currentSkills.Count;
        }

        public void SetFilterText(string _text)
        {
            if (txtInput != null)
            {
                txtInput.OnChangeHandler -= TxtInput_OnChangeHandler;
                filterText = _text;
                txtInput.Text = _text;
                txtInput.OnChangeHandler += TxtInput_OnChangeHandler;
            }
        }

        public void SelectFirstEntry()
        {
            SelectedEntry = skillEntries[0].GetFirstEntry();
        }

        private void XUiC_SkillEntry_OnPress(XUiController _sender, int _mouseButton)
        {
            XUiC_SkillSubEntry xUiC_SkillEntry = (XUiC_SkillSubEntry)_sender;
            if (xUiC_SkillEntry.Skill != null && (xUiC_SkillEntry.Skill.ProgressionClass.Type != ProgressionType.Skill || IsBook))
            {
                SelectedEntry = xUiC_SkillEntry;
                selectName = "";
            }
        }
        
        public void RefreshSkillList()
        {
            UpdateFilteredList();
            RefreshSkillListEntries();
        }

        private void TxtInput_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
        {
            filterText = _text.Trim();
            if (filterText == "")
            {
                if (Category != "attbooks")
                {
                    CategoryList.SetCategoryToFirst();
                }
                else
                {
                    CategoryList.SetCategory(Category);
                }
            }
            else
            {
                if (Category != "attbooks")
                {
                    CategoryList.CurrentCategory = null;
                    Category = "";
                    RefreshSkillList();
                    SelectFirstEntry();
                    WindowGroup.Controller.IsDirty = true;
                }
                else
                {
                    CategoryList.SetCategory(Category);
                }
            }
        }

        private void UpdateFilteredList()
        {
            currentSkills.Clear();
            string category = Category.Trim();
            foreach (ProgressionValue skill in skills)
            {

                ProgressionClass progressionClass = skill?.ProgressionClass;
                if (progressionClass == null || progressionClass.Name == null || progressionClass.Name.EqualsCaseInsensitive("attBooks"))
                {
                    continue;
                }

                //If ProgressionClass NameKey or Localized Name doesn't make filterText, continue
                if (!progressionClass.NameKey.ContainsCaseInsensitive(filterText) && !Localization.Get(progressionClass.NameKey).ContainsCaseInsensitive(filterText) && !string.IsNullOrEmpty(filterText))
                {
                    continue;
                }

                if (IsBook)
                {
                    if(progressionClass.IsBook && !progressionClass.IsPerk && !progressionClass.IsAttribute)
                    {
                        currentSkills.Add(skill);
                    }
                }
                else
                {
                    if((category == "" && !progressionClass.IsSkill && !progressionClass.IsBook)
                        || (category.EqualsCaseInsensitive(progressionClass.Name) && progressionClass.IsAttribute) 
                        || (progressionClass.Parent != null && progressionClass.Parent != progressionClass && progressionClass.IsPerk && category.EqualsCaseInsensitive(progressionClass.Parent.Parent.Name)))
                    {
                        currentSkills.Add(skill);
                    }
                }

                //if (progressionClass != null 
                //    && ((!IsBook && !progressionClass.IsBook) || (IsBook && !progressionClass.IsPerk && !progressionClass.IsAttribute && progressionClass.IsBook)) 
                //    && progressionClass.Name != null 
                //    && (progressionClass.NameKey.ContainsCaseInsensitive(filterText) || Localization.Get(progressionClass.NameKey).ContainsCaseInsensitive(filterText)) 
                //    && !progressionClass.Name.EqualsCaseInsensitive("attBooks") 
                //    && (filterText == "" || !progressionClass.IsSkill || progressionClass.IsBook) 
                //    && (category == ""
                //      || category.EqualsCaseInsensitive(progressionClass.Name)
                //      || (progressionClass.Parent != null && progressionClass.Parent != progressionClass  && progressionClass.IsSkill && category.EqualsCaseInsensitive(progressionClass.Parent.Name)) 
                //      || (progressionClass.Parent != null && progressionClass.Parent != progressionClass  && progressionClass.IsPerk  && category.EqualsCaseInsensitive(progressionClass.Parent.Parent.Name))))
                //{
                //    currentBooks.Add(skill);
                //}
            }

            currentSkills.Sort(ProgressionClass.ListSortOrderComparer.Instance);
        }

        private void RefreshSkillListEntries()
        {
            SelectedEntry = null;
            PopulateSkillEntry(skillEntries, currentSkills, IsBook);

            if (SelectedEntry == null)
            {
                SelectedEntry = skillEntries[0].GetFirstEntry();
                ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = SelectedEntry.Skill;
            }

            if (xui.selectedSkill == null)
            {
                if (selectedEntry != null)
                {
                    ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = selectedEntry.Skill;
                    xui.selectedSkill = selectedEntry.Skill;
                }
            }

            RefreshBindings();
            SkillListWindow.RefreshBindings();
        }

        private void PopulateSkillEntry(XUiC_SkillEntry[] entries, List<ProgressionValue> progressionValues, bool isBook)
        {
            int skillIndex = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                XUiC_SkillEntry entry = entries[i];
                if (skillIndex < progressionValues.Count && progressionValues[skillIndex] != null && Progression.ProgressionClasses.ContainsKey(progressionValues[skillIndex].Name))
                {
                    ProgressionValue skill = progressionValues[skillIndex];
                    ProgressionValue skill2 = null;
                    if (skill.ProgressionClass.IsAttribute)
                    {
                        entry.SetAttributeEntry(skill);
                        skillIndex++;
                    }
                    else
                    {
                        skillIndex++;
                        if(skillIndex < progressionValues.Count && progressionValues[skillIndex] != null 
                            && Progression.ProgressionClasses.ContainsKey(progressionValues[skillIndex].Name)
                            && !progressionValues[skillIndex].ProgressionClass.IsAttribute)
                        {
                           skill2 = progressionValues[skillIndex];
                           skillIndex++;
                        }

                        entry.SetSkillEntries(skill, skill2, isBook);
                    }

                    if(!string.IsNullOrEmpty(selectName)) 
                    {
                        if (skill.ProgressionClass.Name == selectName)
                        {
                            SelectedEntry = entry.GetFirstEntry();
                            ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = SelectedEntry.Skill;
                        }
                        else if (skill2 != null && skill2.ProgressionClass.Name == selectName)
                        {
                            SelectedEntry = entry.GetSecondEntry();
                            ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = SelectedEntry.Skill;
                        }
                    }
                   
                }
                else
                {
                    entry.ClearEntries();
                }
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            skills.Clear();
            xui.playerUI.entityPlayer.Progression.GetDict().CopyValuesTo(skills);
            RefreshSkillList();
        }

        public override void OnClose()
        {
            base.OnClose();
            selectName = "";
        }

        private int GetSkillListRowCount()
        {
            int skillCount = 0;
            int bookCount = 0;

            foreach (ProgressionClass progressionClass in Progression.ProgressionClasses.Values)
            {
                if (progressionClass == null || progressionClass.Name == null || progressionClass.Name.EqualsCaseInsensitive("attBooks"))
                {
                    continue;
                }

                if (progressionClass.IsBook && !progressionClass.IsPerk && !progressionClass.IsAttribute)
                {
                    bookCount++;
                }
                else if ((!progressionClass.IsSkill && !progressionClass.IsBook)
                        && (progressionClass.IsAttribute || (progressionClass.Parent != null && progressionClass.Parent != progressionClass && progressionClass.IsPerk)))
                {
                    skillCount++;
                }
            }
            int rowCount = (MathUtils.Max(skillCount, bookCount) / 2) + 1;

            return rowCount;
        }
    }
}
