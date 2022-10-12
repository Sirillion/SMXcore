using SMXcore.HarmonyPatches;
using System.Collections.Generic;

namespace SMXcore
{
    public class XUiC_SkillList : XUiController
    {
        private List<ProgressionValue> skills = new List<ProgressionValue>();

        private List<ProgressionValue> currentBooks = new List<ProgressionValue>();

        private List<ProgressionValue> currentPerks = new List<ProgressionValue>();

        private List<ProgressionValue> currentAttributes = new List<ProgressionValue>();

        private XUiC_SkillEntry[] bookEntries;

        private XUiC_SkillEntry[] perkEntries;

        private XUiC_SkillEntry[] attributeEntries;

        private string filterText = "";

        private string selectName;

        private XUiC_SkillEntry selectedEntry;

        private XUiC_TextInput txtInput;

        private string category = "";

        public XUiC_SkillEntry SelectedEntry
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

        public XUiC_CategoryList CategoryList { get; set; }

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

            XUiController grid = GetChildById("attributes");
            attributeEntries = grid?.GetChildrenByType<XUiC_SkillEntry>();

            foreach (XUiC_SkillEntry entry in attributeEntries)
            {
                entry.OnPress += XUiC_SkillEntry_OnPress;
            }

            grid = GetChildById("perks");
            perkEntries = grid?.GetChildrenByType<XUiC_SkillEntry>();

            foreach (XUiC_SkillEntry entry in perkEntries)
            {
                entry.OnPress += XUiC_SkillEntry_OnPress;
            }

            grid = GetChildById("books");
            bookEntries = grid?.GetChildrenByType<XUiC_SkillEntry>();

            foreach (XUiC_SkillEntry entry in bookEntries)
            {
                entry.OnPress += XUiC_SkillEntry_OnPress;
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
            return currentBooks.Count;
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
            if (IsBook)
            {
                SelectedEntry = bookEntries[0];
            } 
            else
            {
                SelectedEntry = attributeEntries[0];
            }
        }

        private void XUiC_SkillEntry_OnPress(XUiController _sender, int _mouseButton)
        {
            XUiC_SkillEntry xUiC_SkillEntry = (XUiC_SkillEntry)_sender;
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
            else if (Category != "attbooks")
            {
                CategoryList.SetCategory("");
            }
            else
            {
                CategoryList.SetCategory(Category);
            }
        }

        private void UpdateFilteredList()
        {
            currentAttributes.Clear();
            currentPerks.Clear();
            currentBooks.Clear();
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
                        currentBooks.Add(skill);
                    }
                }
                else
                {
                    if(category.EqualsCaseInsensitive(progressionClass.Name) && progressionClass.IsAttribute)
                    {
                        currentAttributes.Add(skill);
                    }

                    if(progressionClass.Parent != null && progressionClass.Parent != progressionClass && progressionClass.IsPerk && category.EqualsCaseInsensitive(progressionClass.Parent.Parent.Name))
                    {
                        currentPerks.Add(skill);
                    }
                }


                //if (progressionClass != null 
                //    && ((!IsBook && !progressionClass.IsBook) || (IsBook && !progressionClass.IsPerk && !progressionClass.IsAttribute && progressionClass.IsBook)) 
                //    && progressionClass.Name != null 
                //    && (progressionClass.NameKey.ContainsCaseInsensitive(filterText) || Localization.Get(progressionClass.NameKey).ContainsCaseInsensitive(filterText)) 
                //    && !progressionClass.Name.EqualsCaseInsensitive("attBooks") 
                //    && (filterText == "" || !progressionClass.IsSkill || progressionClass.IsBook) 
                //    && (category == "" 
                //        || category.EqualsCaseInsensitive(progressionClass.Name) 
                //        || (progressionClass.Parent != null && progressionClass.Parent != progressionClass  && progressionClass.IsSkill && category.EqualsCaseInsensitive(progressionClass.Parent.Name)) 
                //        || (progressionClass.Parent != null && progressionClass.Parent != progressionClass  && progressionClass.IsPerk  && category.EqualsCaseInsensitive(progressionClass.Parent.Parent.Name))))
                //{
                //    currentBooks.Add(skill);
                //}
            }

            currentBooks.Sort(ProgressionClass.ListSortOrderComparer.Instance);
            currentAttributes.Sort(ProgressionClass.ListSortOrderComparer.Instance);
            currentPerks.Sort(ProgressionClass.ListSortOrderComparer.Instance);
            //if (filterText == "")
            //{
            //    for (int i = 0; i < currentBooks.Count; i++)
            //    {
            //        if (currentBooks[i].ProgressionClass.IsAttribute)
            //        {
            //            for (; i % bookEntries.Length != 0; i++)
            //            {
            //                currentBooks.Insert(i, null);
            //            }
            //        }
            //    }
            //}
        }

        private void RefreshSkillListEntries()
        {
            SelectedEntry = null;

            if (!IsBook)
            {
                //Populate the Attributes and the Perks if not displaying books
                PopulateSkillEntry(attributeEntries, currentAttributes, false);
                PopulateSkillEntry(perkEntries, currentPerks, false);
            }
            else
            {
                //Populates the Skill Books
                PopulateSkillEntry(bookEntries, currentBooks, true);
            }

            if (SelectedEntry == null)
            {
                if (!IsBook)
                {
                    SelectedEntry = attributeEntries[0];
                }
                else
                {
                    SelectedEntry = bookEntries[0];
                }
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
            for (int i = 0; i < entries.Length; i++)
            {
                XUiC_SkillEntry entry = entries[i];
                if (i < progressionValues.Count && progressionValues[i] != null && Progression.ProgressionClasses.ContainsKey(progressionValues[i].Name))
                {
                    entry.Skill = progressionValues[i];
                    entry.IsBook = isBook;
                    entry.ViewComponent.Enabled = true;

                    if ((!string.IsNullOrEmpty(selectName) && selectName == entry.Skill.ProgressionClass.Name)
                        || (xui.selectedSkill != null && entry.Skill.Name == xui.selectedSkill.Name))
                    {
                        SelectedEntry = entry;
                    }
                }
                else
                {
                    entry.Skill = null;
                    entry.ViewComponent.Enabled = false;
                }
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            skills.Clear();
            xui.playerUI.entityPlayer.Progression.ProgressionValues.Dict.CopyValuesTo(skills);
            RefreshSkillList();
        }

        public override void OnClose()
        {
            base.OnClose();
            selectName = "";
        }
    }
}
