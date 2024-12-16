using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_SkillCraftingInfoWindow : XUiC_InfoWindow
    {
        public XUiC_ItemActionList actionItemList;

        public int hiddenEntriesWithPaging = 1;

        public readonly List<XUiC_SkillCraftingInfoEntry> levelEntries = new List<XUiC_SkillCraftingInfoEntry>();

        public XUiC_Paging pager;

        public XUiC_SkillCraftingInfoEntry selectedEntry;

        public int skillsPerPage;
        public ProgressionClass.DisplayData hoveredLevel;
        public ProgressionClass.DisplayData selectedLevel;

        public readonly CachedStringFormatterFloat skillLevelFormatter = new CachedStringFormatterFloat();
        public readonly CachedStringFormatterFloat maxSkillLevelFormatter = new CachedStringFormatterFloat();
        public readonly CachedStringFormatter<string, float> attributeSetValueFormatter = new CachedStringFormatter<string, float>((string _s, float _f) => _s + ": " + _f.ToCultureInvariantString("0.#"));

        public XUiC_SkillCraftingInfoEntry SelectedEntry
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
                }
            }
        }

        public ProgressionValue CurrentSkill
        {
            [PublicizedFrom(EAccessModifier.Private)]
            get
            {
                if (base.xui.selectedSkill == null || !base.xui.selectedSkill.ProgressionClass.IsCrafting)
                {
                    return null;
                }

                return base.xui.selectedSkill;
            }
        }

        public ProgressionClass.DisplayData HoveredData
        {
            get
            {
                return hoveredLevel;
            }
            set
            {
                if (hoveredLevel != value)
                {
                    hoveredLevel = value;
                    RefreshBindings();
                }
            }
        }

        public ProgressionClass.DisplayData SelectedData
        {
            get
            {
                return selectedLevel;
            }
            set
            {
                if (selectedLevel != value)
                {
                    selectedLevel = value;
                    RefreshBindings();
                }
            }
        }

        public ProgressionClass.DisplayData CurrentData
        {
            get
            {
                if (hoveredLevel != null)
                {
                    return hoveredLevel;
                }

                return selectedLevel;
            }
        }

        public override void Init()
        {
            base.Init();
            GetChildrenByType(levelEntries);
            int num = 1;
            foreach (XUiC_SkillCraftingInfoEntry levelEntry in levelEntries)
            {
                levelEntry.ListIndex = num - 1;
                levelEntry.Data = null;
                levelEntry.HiddenEntriesWithPaging = hiddenEntriesWithPaging;
                levelEntry.MaxEntriesWithoutPaging = levelEntries.Count;
                levelEntry.OnHover += Entry_OnHover;
                levelEntry.OnPress += Entry_OnPress;
            }

            for (int i = 0; i < 14; i++)
            {
                XUiController childById = GetChildById($"itemIcon{i + 1}");
                childById.CustomData = i;
                childById.OnPress += Image_OnPress;
            }

            actionItemList = GetChildByType<XUiC_ItemActionList>();
            skillsPerPage = levelEntries.Count - hiddenEntriesWithPaging;
            pager = GetChildByType<XUiC_Paging>();
            if (pager != null)
            {
                pager.OnPageChanged += Pager_OnPageChanged;
            }
        }

        public void Image_OnPress(XUiController _sender, int _mouseButton)
        {
            int index = (int)_sender.CustomData;
            XUi xUi = _sender.xui;
            if (CurrentData == null || CurrentData.GetUnlockItemRecipes(index) == null)
            {
                return;
            }

            xUi.playerUI.windowManager.CloseIfOpen("looting");
            List<XUiC_RecipeList> childrenByType = xUi.GetChildrenByType<XUiC_RecipeList>();
            XUiC_RecipeList xUiC_RecipeList = null;
            for (int i = 0; i < childrenByType.Count; i++)
            {
                if (childrenByType[i].WindowGroup != null && childrenByType[i].WindowGroup.isShowing)
                {
                    xUiC_RecipeList = childrenByType[i];
                    break;
                }
            }

            if (xUiC_RecipeList == null)
            {
                XUiC_WindowSelector.OpenSelectorAndWindow(xUi.playerUI.entityPlayer, "crafting");
                xUiC_RecipeList = xUi.GetChildByType<XUiC_RecipeList>();
            }

            xUiC_RecipeList?.SetRecipeDataByItems(CurrentData.GetUnlockItemRecipes(index));
        }

        public void Entry_OnPress(XUiController _sender, int _mouseButton)
        {
            XUiC_SkillCraftingInfoEntry xUiC_SkillCraftingInfoEntry = _sender as XUiC_SkillCraftingInfoEntry;
            if (xUiC_SkillCraftingInfoEntry == null)
            {
                xUiC_SkillCraftingInfoEntry = _sender.Parent as XUiC_SkillCraftingInfoEntry;
            }

            if (xUiC_SkillCraftingInfoEntry != null)
            {
                if (SelectedData != xUiC_SkillCraftingInfoEntry.Data && xUiC_SkillCraftingInfoEntry.Data != null)
                {
                    SelectedEntry = xUiC_SkillCraftingInfoEntry;
                    SelectedData = xUiC_SkillCraftingInfoEntry.Data;
                }
                else
                {
                    SelectedEntry = null;
                    SelectedData = null;
                }
            }
            else
            {
                SelectedEntry = null;
                SelectedData = null;
            }
        }

        public void Entry_OnHover(XUiController _sender, bool _isOver)
        {
            XUiC_SkillCraftingInfoEntry xUiC_SkillCraftingInfoEntry = _sender as XUiC_SkillCraftingInfoEntry;
            if (xUiC_SkillCraftingInfoEntry == null)
            {
                xUiC_SkillCraftingInfoEntry = _sender.Parent as XUiC_SkillCraftingInfoEntry;
            }

            if (_isOver && xUiC_SkillCraftingInfoEntry != null)
            {
                HoveredData = xUiC_SkillCraftingInfoEntry.Data;
            }
            else
            {
                HoveredData = null;
            }
        }

        public void SkillChanged()
        {
            pager?.SetLastPageByElementsAndPageLength((CurrentSkill != null && CurrentSkill.ProgressionClass.MaxLevel > levelEntries.Count) ? (CurrentSkill.ProgressionClass.MaxLevel - 1) : 0, skillsPerPage);
            pager?.Reset();
            IsDirty = true;
            SelectedData = null;
            SelectedEntry = null;
        }

        public void UpdateSkill()
        {
            if (CurrentSkill != null && actionItemList != null)
            {
                actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
            }

            int num = (pager?.GetPage() ?? 0) * skillsPerPage;
            ProgressionClass progressionClass = ((CurrentSkill != null) ? CurrentSkill.ProgressionClass : null);
            if (progressionClass != null && progressionClass.DisplayDataList != null)
            {
                XUiC_SkillEntry entryForSkill = windowGroup.Controller.GetChildByType<XUiC_SkillListWindow>().GetEntryForSkill(CurrentSkill);
                {
                    foreach (XUiC_SkillCraftingInfoEntry levelEntry in levelEntries)
                    {
                        levelEntry.Data = ((progressionClass.DisplayDataList.Count > num) ? progressionClass.DisplayDataList[num] : null);
                        levelEntry.IsDirty = true;
                        if (entryForSkill != null)
                        {
                            levelEntry.ViewComponent.NavLeftTarget = entryForSkill.ViewComponent;
                        }

                        num++;
                    }

                    return;
                }
            }

            foreach (XUiC_SkillCraftingInfoEntry levelEntry2 in levelEntries)
            {
                levelEntry2.Data = null;
                levelEntry2.IsDirty = true;
            }
        }

        public void Pager_OnPageChanged()
        {
            IsDirty = true;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            if (actionItemList != null)
            {
                actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
            }

            XUiEventManager.Instance.OnSkillExperienceAdded += Current_OnSkillExperienceAdded;
            IsDirty = true;
        }

        public override void OnClose()
        {
            base.OnClose();
            XUiEventManager.Instance.OnSkillExperienceAdded -= Current_OnSkillExperienceAdded;
        }

        public override void Update(float _dt)
        {
            if (IsDirty)
            {
                IsDirty = false;
                UpdateSkill();
                RefreshBindings(IsDirty);
            }

            base.Update(_dt);
        }

        public void Current_OnSkillExperienceAdded(ProgressionValue _changedSkill, int _newXp)
        {
            if (CurrentSkill == _changedSkill)
            {
                IsDirty = true;
            }
        }

        public override bool ParseAttribute(string _name, string _value, XUiController _parent)
        {
            if (_name == "hidden_entries_with_paging")
            {
                hiddenEntriesWithPaging = StringParsers.ParseSInt32(_value);
                foreach (XUiC_SkillCraftingInfoEntry levelEntry in levelEntries)
                {
                    if (levelEntry != null)
                    {
                        levelEntry.HiddenEntriesWithPaging = hiddenEntriesWithPaging;
                    }
                }

                skillsPerPage = levelEntries.Count - hiddenEntriesWithPaging;
                return true;
            }

            return base.ParseAttribute(_name, _value, _parent);
        }

        public override bool GetBindingValue(ref string _value, string _bindingName)
        {
            EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
            switch (_bindingName)
            {
                case "alwaysfalse":
                    _value = "false";
                    return true;
                case "groupicon":
                    _value = ((CurrentSkill != null) ? CurrentSkill.ProgressionClass.Icon : "ui_game_symbol_skills");
                    return true;
                case "groupname":
                    _value = ((CurrentSkill != null) ? Localization.Get(CurrentSkill.ProgressionClass.NameKey) : "Skill Info");
                    return true;
                case "groupdescription":
                    _value = ((CurrentSkill != null) ? Localization.Get(CurrentSkill.ProgressionClass.DescKey) : "");
                    return true;
                case "detailsdescription":
                    _value = "";
                    return true;
                case "skillLevel":
                    _value = ((CurrentSkill != null) ? skillLevelFormatter.Format(ProgressionClass.GetCalculatedLevel(entityPlayer, CurrentSkill)) : "0");
                    return true;
                case "maxSkillLevel":
                    _value = ((CurrentSkill != null) ? maxSkillLevelFormatter.Format(ProgressionClass.GetCalculatedMaxLevel(entityPlayer, CurrentSkill)) : "0");
                    return true;
                case "currentlevel":
                    _value = Localization.Get("xuiSkillLevel");
                    return true;
                case "showPaging":
                    _value = "false";
                    return true;
                default:
                    if (_bindingName.StartsWith("unlock_icon_atlas"))
                    {
                        if (CurrentData != null)
                        {
                            int index = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon_atlas", "")) - 1;
                            _value = CurrentData.GetUnlockItemIconAtlas(entityPlayer, index);
                        }
                        else
                        {
                            _value = "ItemIconAtlas";
                        }

                        return true;
                    }

                    if (_bindingName.StartsWith("unlock_icon_locked"))
                    {
                        if (CurrentData != null)
                        {
                            int index2 = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon_locked", "")) - 1;
                            _value = CurrentData.GetUnlockItemLocked(entityPlayer, index2).ToString();
                        }
                        else
                        {
                            _value = "false";
                        }

                        return true;
                    }

                    if (_bindingName.StartsWith("unlock_icon_tooltip"))
                    {
                        if (CurrentData != null)
                        {
                            int index3 = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon_tooltip", "")) - 1;
                            _value = CurrentData.GetUnlockItemName(index3);
                        }
                        else
                        {
                            _value = "";
                        }

                        return true;
                    }

                    if (_bindingName.StartsWith("unlock_icon"))
                    {
                        if (CurrentData != null)
                        {
                            int index4 = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon", "")) - 1;
                            _value = CurrentData.GetUnlockItemIconName(index4);
                        }
                        else
                        {
                            _value = "";
                        }

                        return true;
                    }

                    return false;
            }
        }
    }
}
