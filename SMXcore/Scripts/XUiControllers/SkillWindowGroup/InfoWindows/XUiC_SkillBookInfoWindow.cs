using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_SkillBookInfoWindow : XUiC_InfoWindow
    {
        public XUiC_ItemActionList actionItemList;

        public int hiddenEntriesWithPaging = 1;

        public readonly List<XUiC_SkillBookLevel> perkEntries = new List<XUiC_SkillBookLevel>();

        public XUiC_Paging pager;
        public int skillsPerPage;

        public List<ProgressionValue> perkList = new List<ProgressionValue>();

        public ProgressionValue hoveredPerk;

        public readonly CachedStringFormatterFloat skillLevelFormatter = new CachedStringFormatterFloat();
        public readonly CachedStringFormatterFloat maxSkillLevelFormatter = new CachedStringFormatterFloat();
        public readonly CachedStringFormatter<int> buyCostFormatter = new CachedStringFormatter<int>((int _i) => _i + " " + Localization.Get("xuiSkillPoints"));
        public readonly CachedStringFormatter<int> expCostFormatter = new CachedStringFormatter<int>((int _i) => _i + " " + Localization.Get("RewardExp_keyword"));

        public ProgressionValue CurrentSkill
        {
            [PublicizedFrom(EAccessModifier.Private)]
            get
            {
                if (xui.selectedSkill == null || !xui.selectedSkill.ProgressionClass.IsBookGroup)
                {
                    return null;
                }

                return xui.selectedSkill;
            }
        }

        public ProgressionValue HoveredPerk
        {
            get
            {
                return hoveredPerk;
            }
            set
            {
                if (hoveredPerk != value)
                {
                    hoveredPerk = value;
                    RefreshBindings();
                }
            }
        }

        public override void Init()
        {
            base.Init();
            GetChildrenByType(perkEntries);
            int num = 1;
            foreach (XUiC_SkillBookLevel perkEntry in perkEntries)
            {
                perkEntry.ListIndex = num - 1;
                perkEntry.HiddenEntriesWithPaging = hiddenEntriesWithPaging;
                perkEntry.MaxEntriesWithoutPaging = perkEntries.Count;
                perkEntry.OnScroll += Entry_OnScroll;
            }

            actionItemList = GetChildByType<XUiC_ItemActionList>();
            skillsPerPage = perkEntries.Count - hiddenEntriesWithPaging;
        }

        public void SkillChanged()
        {
            IsDirty = true;
        }

        public void UpdateSkill()
        {
            if (CurrentSkill != null && actionItemList != null)
            {
                actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
            }

            if (CurrentSkill != null)
            {
                base.xui.playerUI.entityPlayer.Progression.GetPerkList(perkList, CurrentSkill.Name);
            }

            XUiC_SkillEntry entryForSkill = windowGroup.Controller.GetChildByType<XUiC_SkillListWindow>().GetEntryForSkill(CurrentSkill);
            int num = 0;
            foreach (XUiC_SkillBookLevel perkEntry in perkEntries)
            {
                if (num < perkList.Count)
                {
                    perkEntry.Perk = perkList[num];
                    perkEntry.Volume = num + 1;
                    perkEntry.OnHover += Entry_OnHover;
                    perkEntry.CompletionReward = num == perkList.Count - 1;
                    if (entryForSkill != null)
                    {
                        perkEntry.ViewComponent.NavLeftTarget = entryForSkill.ViewComponent;
                    }
                }
                else
                {
                    perkEntry.Perk = null;
                    perkEntry.Volume = -1;
                    perkEntry.OnHover -= Entry_OnHover;
                    perkEntry.CompletionReward = false;
                }

                num++;
            }
        }

        public void Entry_OnHover(XUiController _sender, bool _isOver)
        {
            XUiC_SkillBookLevel xUiC_SkillBookLevel = _sender as XUiC_SkillBookLevel;
            if (_isOver && xUiC_SkillBookLevel != null)
            {
                HoveredPerk = xUiC_SkillBookLevel.Perk;
            }
            else
            {
                HoveredPerk = null;
            }
        }

        public void Pager_OnPageChanged()
        {
            IsDirty = true;
        }

        public void Entry_OnScroll(XUiController _sender, float _delta)
        {
            if (_delta > 0f)
            {
                pager?.PageDown();
            }
            else
            {
                pager?.PageUp();
            }
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
                foreach (XUiC_SkillBookLevel perkEntry in perkEntries)
                {
                    if (perkEntry != null)
                    {
                        perkEntry.HiddenEntriesWithPaging = hiddenEntriesWithPaging;
                    }
                }

                skillsPerPage = perkEntries.Count - hiddenEntriesWithPaging;
                return true;
            }

            return base.ParseAttribute(_name, _value, _parent);
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
            switch (bindingName)
            {
                case "groupicon":
                    value = ((CurrentSkill != null) ? CurrentSkill.ProgressionClass.Icon : "ui_game_symbol_skills");
                    return true;
                case "groupname":
                    value = ((CurrentSkill != null) ? Localization.Get(CurrentSkill.ProgressionClass.NameKey) : "Skill Info");
                    return true;
                case "groupdescription":
                    if (CurrentSkill != null)
                    {
                        value = Localization.Get(CurrentSkill.ProgressionClass.DescKey);
                    }
                    else
                    {
                        value = "";
                    }

                    return true;
                case "detailsdescription":
                    if (CurrentSkill != null)
                    {
                        if (hoveredPerk != null)
                        {
                            if (string.IsNullOrEmpty(hoveredPerk.ProgressionClass.LongDescKey))
                            {
                                value = Localization.Get(hoveredPerk.ProgressionClass.DescKey);
                            }
                            else
                            {
                                value = Localization.Get(hoveredPerk.ProgressionClass.LongDescKey);
                            }
                        }
                        else
                        {
                            value = Localization.Get(CurrentSkill.ProgressionClass.LongDescKey);
                        }
                    }
                    else
                    {
                        value = "";
                    }

                    return true;
                case "skillLevel":
                    value = ((CurrentSkill != null) ? skillLevelFormatter.Format(ProgressionClass.GetCalculatedLevel(entityPlayer, CurrentSkill)) : "0");
                    return true;
                case "maxSkillLevel":
                    value = ((CurrentSkill != null) ? maxSkillLevelFormatter.Format(ProgressionClass.GetCalculatedMaxLevel(entityPlayer, CurrentSkill)) : "0");
                    return true;
                case "currentlevel":
                    value = Localization.Get("xuiSkillLevel");
                    return true;
                case "showPaging":
                    value = "false";
                    return true;
                default:
                    return false;
            }
        }
    }
}
