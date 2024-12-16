using Quartz;
using SMXcore.HarmonyPatches;
using System;
using System.Collections.Generic;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.

namespace SMXcore
{
    public class XUiC_CraftingSkillList : XUiController
    {
        private List<ProgressionValue> skills = new List<ProgressionValue>();
        private List<ProgressionValue> currentCraftingSkills = new List<ProgressionValue>();

        private XUiC_BookSkillEntry[] craftingSkillEntries;

        private string selectName;

        private XUiC_SkillEntry selectedEntry;

        private XUiC_TextInput txtInput;


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

        public XUiC_SkillListWindow SkillListWindow { get; set; }

        public override void Init()
        {
            base.Init();

            craftingSkillEntries = GetChildrenByType<XUiC_BookSkillEntry>();
            foreach(XUiC_SkillEntry entry in craftingSkillEntries)
            {
                entry.OnPress += XUiC_SkillEntry_OnPress;
                entry.DisplayType = ProgressionClass.DisplayTypes.Book;
            }

        }

        public void SelectFirstEntry()
        {
            SelectedEntry = craftingSkillEntries[0];
        }

        private void XUiC_SkillEntry_OnPress(XUiController sender, int _mouseButton)
        {
            XUiC_SkillEntry skillEntry = sender as XUiC_SkillEntry;
            if (skillEntry.Skill != null)
            {
                SelectedEntry = skillEntry;
                selectName = "";
            }
        }

        internal int GetActiveCount()
        {
            return currentCraftingSkills.Count;
        }

        public void RefreshSkillList()
        {
            UpdateSkillLists();
            RefreshSkillListEntries();
        }

        private void UpdateSkillLists()
        {
            currentCraftingSkills.Clear();

            foreach (ProgressionValue skill in skills)
            {
                ProgressionClass progressionClass = skill?.ProgressionClass;
                if (progressionClass == null || progressionClass.Name == null || !progressionClass.ValidDisplay(ProgressionClass.DisplayTypes.Crafting))
                {
                    continue;
                }

                if (progressionClass.IsCrafting)
                {
                    currentCraftingSkills.Add(skill);
                }
            }

            currentCraftingSkills.Sort(ProgressionClass.ListSortOrderComparer.Instance);
        }

        private void RefreshSkillListEntries()
        {
            XUiView bookInfoViewComponent = ((XUiC_SkillWindowGroup)WindowGroup.Controller).skillCraftingInfoWindow.GetChildById("0").ViewComponent;

            SelectedEntry = null;
            PopulateSkillEntry(craftingSkillEntries, currentCraftingSkills, bookInfoViewComponent);

            if (SelectedEntry == null)
            {
                SelectedEntry = craftingSkillEntries[0];
                SelectedEntry.RefreshBindings();
                ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = SelectedEntry.Skill;
            }

            //if (xui.selectedSkill == null)
            //{
            //    if (selectedEntry != null)
            //    {
            //        ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = selectedEntry.Skill;
            //        xui.selectedSkill = selectedEntry.Skill;
            //    }
            //}

            RefreshBindings();
            SkillListWindow.RefreshBindings();
        }

        private void PopulateSkillEntry(XUiC_SkillEntry[] entries, List<ProgressionValue> progressionValues, XUiView navRightTarget)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                XUiC_SkillEntry entry = entries[i];
                if (i < progressionValues.Count && progressionValues[i] != null && Progression.ProgressionClasses.ContainsKey(progressionValues[i].Name))
                {
                    ProgressionValue skill = progressionValues[i];
                    entry.Skill = skill;

                    if (!string.IsNullOrEmpty(selectName) && skill.ProgressionClass.Name == selectName)
                    {
                        SelectedEntry = entry;
                        ((XUiC_SkillWindowGroup)WindowGroup.Controller).CurrentSkill = SelectedEntry.Skill;
                    } 
                    else
                    {
                        entry.IsSelected = false;
                    }
                    entry.ViewComponent.Enabled = true;
                    entry.ViewComponent.NavRightTarget = navRightTarget;
                    entry.RefreshBindings();

                }
                else
                {
                    entry.Skill = null;
                    entry.IsSelected = false;
                    entry.ViewComponent.Enabled = false;
                    entry.RefreshBindings();

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

        public XUiC_SkillEntry GetEntryForSkill(ProgressionValue skill)
        {
            XUiC_SkillEntry[] skillEntries = new XUiC_SkillEntry[0];
            if(skill.ProgressionClass.IsCrafting)
            {
                skillEntries = craftingSkillEntries;
            }

            foreach (XUiC_SkillEntry xuiC_SkillEntry in skillEntries)
            {
                if (xuiC_SkillEntry.Skill == skill)
                {
                    return xuiC_SkillEntry;
                }
            }
            return null;
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "craftingskillcount":
                    value = GetMaxItemCount().ToString();
                    return true;

                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        private int GetMaxItemCount()
        {
            int count = 0;

            foreach (ProgressionClass progressionClass in Progression.ProgressionClasses.Values)
            {
                if (progressionClass == null || progressionClass.Name == null || !progressionClass.ValidDisplay(ProgressionClass.DisplayTypes.Crafting))
                {
                    continue;
                }

                if (progressionClass.IsCrafting)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
