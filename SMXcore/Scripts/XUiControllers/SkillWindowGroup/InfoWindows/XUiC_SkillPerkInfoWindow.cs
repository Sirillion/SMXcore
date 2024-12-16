using Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_SkillPerkInfoWindow : XUiC_InfoWindow
    {
        public XUiC_ItemActionList actionItemList;

        public int hiddenEntriesWithPaging = 1;

        public readonly List<XUiC_SkillPerkLevel> levelEntries = new List<XUiC_SkillPerkLevel>();

        public XUiC_Paging pager;
        public int skillsPerPage;

        public int hoveredLevel = -1;

        public readonly CachedStringFormatterFloat skillLevelFormatter = new CachedStringFormatterFloat(null);
        public readonly CachedStringFormatterFloat maxSkillLevelFormatter = new CachedStringFormatterFloat(null);
        public readonly CachedStringFormatter<int> buyCostFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString() + " " + Localization.Get("xuiSkillPoints", false));
        public readonly CachedStringFormatter<int> expCostFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString() + " " + Localization.Get("RewardExp_keyword", false));
        public readonly CachedStringFormatter<string, float, bool> attributeSubtractionFormatter = new CachedStringFormatter<string, float, bool>((string _s, float _f, bool _b) => _s + ": " + _f.ToCultureInvariantString("0.#") + (_b ? "%" : ""));
        public readonly CachedStringFormatter<string, float> attributeSetValueFormatter = new CachedStringFormatter<string, float>((string _s, float _f) => _s + ": " + _f.ToCultureInvariantString("0.#"));

        public readonly StringBuilder effectsStringBuilder = new StringBuilder();

        public ProgressionValue CurrentSkill
        {
            get
            {
                if (xui.selectedSkill == null || !xui.selectedSkill.ProgressionClass.IsPerk)
                {
                    return null;
                }
                return xui.selectedSkill;
            }
        }
        public int HoveredLevel
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
                    base.RefreshBindings(false);
                }
            }
        }

        public override void Init()
        {
            base.Init();
            GetChildrenByType<XUiC_SkillPerkLevel>(levelEntries);
            int num = 1;
            foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in levelEntries)
            {
                xuiC_SkillPerkLevel.ListIndex = num - 1;
                xuiC_SkillPerkLevel.Level = num++;
                xuiC_SkillPerkLevel.HiddenEntriesWithPaging = hiddenEntriesWithPaging;
                xuiC_SkillPerkLevel.MaxEntriesWithoutPaging = levelEntries.Count;
                xuiC_SkillPerkLevel.OnScroll += Entry_OnScroll;
                xuiC_SkillPerkLevel.OnHover += Entry_OnHover;
                xuiC_SkillPerkLevel.btnBuy.Controller.OnHover += Entry_OnHover;
            }
            actionItemList = GetChildByType<XUiC_ItemActionList>();
            skillsPerPage = levelEntries.Count - hiddenEntriesWithPaging;
            pager = GetChildByType<XUiC_Paging>();
            if (pager != null)
            {
                pager.OnPageChanged += Pager_OnPageChanged;
            }
        }

        public void Entry_OnHover(XUiController _sender, bool _isOver)
        {
            XUiC_SkillPerkLevel xuiC_SkillPerkLevel = _sender as XUiC_SkillPerkLevel;
            if (xuiC_SkillPerkLevel == null)
            {
                xuiC_SkillPerkLevel = _sender.Parent as XUiC_SkillPerkLevel;
            }
            if (_isOver && xuiC_SkillPerkLevel != null)
            {
                HoveredLevel = xuiC_SkillPerkLevel.Level;
                return;
            }
            HoveredLevel = -1;
        }

        public void SkillChanged()
        {
            XUiC_Paging xuiC_Paging = pager;
            if (xuiC_Paging != null)
            {
                xuiC_Paging.SetLastPageByElementsAndPageLength((CurrentSkill != null && CurrentSkill.ProgressionClass.MaxLevel > levelEntries.Count) ? (CurrentSkill.ProgressionClass.MaxLevel - 1) : 0, skillsPerPage);
            }
            XUiC_Paging xuiC_Paging2 = pager;
            if (xuiC_Paging2 != null)
            {
                xuiC_Paging2.Reset();
            }
            IsDirty = true;
        }

        public void UpdateSkill()
        {
            if (CurrentSkill != null && actionItemList != null)
            {
                actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
            }
            XUiC_SkillEntry entryForSkill = windowGroup.Controller.GetChildByType<XUiC_SkillListWindow>().GetEntryForSkill(CurrentSkill);
            XUiC_Paging xuiC_Paging = pager;
            int num = ((xuiC_Paging != null) ? xuiC_Paging.GetPage() : 0) * skillsPerPage + 1;
            foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in levelEntries)
            {
                xuiC_SkillPerkLevel.Level = num++;
                xuiC_SkillPerkLevel.IsDirty = true;
                if (entryForSkill != null)
                {
                    xuiC_SkillPerkLevel.btnBuy.NavLeftTarget = entryForSkill.ViewComponent;
                }
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
                XUiC_Paging xuiC_Paging = pager;
                if (xuiC_Paging == null)
                {
                    return;
                }
                xuiC_Paging.PageDown();
                return;
            }
            else
            {
                XUiC_Paging xuiC_Paging2 = pager;
                if (xuiC_Paging2 == null)
                {
                    return;
                }
                xuiC_Paging2.PageUp();
                return;
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
                base.RefreshBindings(IsDirty);
            }
            if (base.ViewComponent.UiTransform.gameObject.activeInHierarchy && CurrentSkill != null && !xui.playerUI.windowManager.IsInputActive() && ((PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && xui.playerUI.playerInput.GUIActions.Inspect.WasPressed) || (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && xui.playerUI.playerInput.GUIActions.DPad_Up.WasPressed)))
            {
                foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in levelEntries)
                {
                    if (xuiC_SkillPerkLevel.CurrentSkill != null && xuiC_SkillPerkLevel.Level == CurrentSkill.Level + 1)
                    {
                        xuiC_SkillPerkLevel.btnBuy.Controller.Pressed(-1);
                        break;
                    }
                }
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
                hiddenEntriesWithPaging = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
                foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in levelEntries)
                {
                    if (xuiC_SkillPerkLevel != null)
                    {
                        xuiC_SkillPerkLevel.HiddenEntriesWithPaging = hiddenEntriesWithPaging;
                    }
                }
                skillsPerPage = levelEntries.Count - hiddenEntriesWithPaging;
                return true;
            }
            return base.ParseAttribute(_name, _value, _parent);
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            EntityPlayerLocal entityPlayer = xui.playerUI.entityPlayer;
            switch (bindingName)
            {
                case "alwaysfalse":
                    value = "false";
                    return true;
                case "maxSkillLevel":
                    value = ((CurrentSkill != null) ? maxSkillLevelFormatter.Format(ProgressionClass.GetCalculatedMaxLevel(entityPlayer, CurrentSkill)) : "0");
                    return true;
                case "skillLevel":
                    value = ((CurrentSkill != null) ? skillLevelFormatter.Format(ProgressionClass.GetCalculatedLevel(entityPlayer, CurrentSkill)) : "0");
                    return true;
                case "buycost":
                    value = "-- PTS";
                    if (CurrentSkill != null && CurrentSkill.Level < CurrentSkill.ProgressionClass.MaxLevel)
                    {
                        if (CurrentSkill.ProgressionClass.CurrencyType == ProgressionCurrencyType.SP)
                        {
                            value = buyCostFormatter.Format(CurrentSkill.ProgressionClass.CalculatedCostForLevel(CurrentSkill.CalculatedLevel(entityPlayer) + 1));
                        }
                        else
                        {
                            value = expCostFormatter.Format((int)((1f - CurrentSkill.PercToNextLevel) * (float)CurrentSkill.ProgressionClass.CalculatedCostForLevel(CurrentSkill.CalculatedLevel(entityPlayer) + 1)));
                        }
                    }
                    return true;
                case "currentlevel":
                    value = Localization.Get("xuiSkillLevel", false);
                    return true;
                case "showPaging":
                    value = (CurrentSkill != null && CurrentSkill.ProgressionClass.MaxLevel > levelEntries.Count).ToString();
                    return true;
                case "groupdescription":
                    value = ((CurrentSkill != null) ? Localization.Get(CurrentSkill.ProgressionClass.DescKey, false) : "");
                    return true;
                case "detailsdescription":
                    if (CurrentSkill != null && hoveredLevel != -1 && CurrentSkill.ProgressionClass.MaxLevel >= hoveredLevel)
                    {
                        using (List<MinEffectGroup>.Enumerator enumerator = CurrentSkill.ProgressionClass.Effects.EffectGroups.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                MinEffectGroup minEffectGroup = enumerator.Current;
                                if (minEffectGroup.EffectDescriptions != null)
                                {
                                    for (int i = 0; i < minEffectGroup.EffectDescriptions.Count; i++)
                                    {
                                        if (hoveredLevel >= minEffectGroup.EffectDescriptions[i].MinLevel && hoveredLevel <= minEffectGroup.EffectDescriptions[i].MaxLevel)
                                        {
                                            value = ((!string.IsNullOrEmpty(minEffectGroup.EffectDescriptions[i].LongDescription)) ? minEffectGroup.EffectDescriptions[i].LongDescription : minEffectGroup.EffectDescriptions[i].Description);
                                            return true;
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                    }
                    value = "";
                    return true;
                case "groupicon":
                    value = ((CurrentSkill != null) ? CurrentSkill.ProgressionClass.Icon : "ui_game_symbol_skills");
                    return true;
                case "groupname":
                    value = ((CurrentSkill != null) ? Localization.Get(CurrentSkill.ProgressionClass.NameKey, false) : "Skill Info");
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);

            }
        }
    }
}
