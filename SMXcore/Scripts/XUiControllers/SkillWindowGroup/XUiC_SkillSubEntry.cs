using UnityEngine;

//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Changes a bit of the Behaviour of the SkillEntry

namespace SMXcore
{
    public class XUiC_SkillSubEntry : XUiController
    {
        private ProgressionValue currentSkill;

        private string enabledColor;

        private string disabledColor;

        private string rowColor;

        private string hoverColor;

        private string selectedColor = "160,160,160";

        private bool isSelected;

        public bool IsHovered;

        private bool isBook;

        private readonly CachedStringFormatter<int, int, int> groupLevelFormatter = new CachedStringFormatter<int, int, int>((int _i3, int _i1, int _i2) => (_i1 >= _i3) ? ((_i1 <= _i3) ? (_i1 + "/" + _i2) : ("[11cc11]" + _i1 + "[-]/" + _i2)) : ("[cc1111]" + _i1 + "[-]/" + _i2));

        private readonly CachedStringFormatter<int> groupPointCostFormatter = new CachedStringFormatter<int>((int _i) => string.Format("{0} {1}", _i, (_i != 1) ? Localization.Get("xuiSkillPoints") : Localization.Get("xuiSkillPoint")));

        private readonly CachedStringFormatter<float> skillPercentThisLevelFormatter = new CachedStringFormatter<float>((float _i) => _i.ToCultureInvariantString());

        public ProgressionValue Skill
        {
            get
            {
                return currentSkill;
            }
            set
            {
                if(currentSkill != value)
                {
                    currentSkill = value;
                    IsDirty = true;
                    IsHovered = false;
                    isSelected = false;

                    RefreshBindings(_forceAll: true);
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    IsDirty = true;
                    isSelected = value;
                }
            }
        }

        public bool IsBook
        {
            get
            {
                return isBook;
            }
            set
            {
                isBook = value;
            }
        }

        public override void Update(float _dt)
        {
            base.Update(_dt);
            if (IsDirty)
            {
                IsDirty = false;
                RefreshBindings();
            }
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "groupicon":
                    value = GetGroupIcon();
                    return true;
                case "groupname":
                    value = ((currentSkill != null) ? Localization.Get(currentSkill.ProgressionClass.NameKey) : "");
                    return true;
                case "grouplevel":
                    value = GetGroupLevel();
                    return true;
                case "islocked":
                    value = ((currentSkill != null) ? (currentSkill.CalculatedLevel(xui.playerUI.entityPlayer) > currentSkill.CalculatedMaxLevel(xui.playerUI.entityPlayer)).ToString() : "false");
                    return true;
                case "isnotlocked":
                    value = ((currentSkill != null) ? (currentSkill.CalculatedLevel(xui.playerUI.entityPlayer) <= currentSkill.CalculatedMaxLevel(xui.playerUI.entityPlayer)).ToString() : "true");
                    return true;
                case "grouptypeicon":
                    value = GetGroupTypeIcon();
                    return true;
                case "grouppointcost":
                    value = GetGroupPointCost();
                    return true;
                case "canpurchase":
                    if (isBook)
                    {
                        value = "true";
                    }
                    else
                    {
                        value = CanPurchaseSkill().ToString();
                    }
                    return true;
                case "cannotpurchase":
                    if (isBook)
                    {
                        value = "false";
                    }
                    else
                    {
                        value = (!CanPurchaseSkill()).ToString();
                    }
                    return true;
                case "requiredskill":
                    {
                        string text = "NA";
                        if (currentSkill != null)
                        {
                            text = currentSkill.ProgressionClass.NameKey;
                        }

                        value = text;
                        return true;
                    }
                case "statuscolor":
                    value = ((currentSkill == null) ? disabledColor : ((currentSkill.CalculatedMaxLevel(xui.playerUI.entityPlayer) == 0) ? disabledColor : enabledColor));
                    return true;
                case "hasskill":
                    value = (currentSkill != null).ToString();
                    return true;
                case "ishighlighted":
                    value = (IsHovered || IsSelected).ToString();
                    return true;
                case "isnothighlighted":
                    value = (!IsHovered && !IsSelected).ToString();
                    return true;
                case "rowstatecolor":
                    value = (IsSelected ? selectedColor : (IsHovered ? hoverColor : rowColor));
                    return true;
                case "rowstatesprite":
                    value = (IsSelected ? "ui_game_select_row" : "menu_empty");
                    return true;
                case "skillpercentthislevel":
                    value = ((currentSkill != null) ? skillPercentThisLevelFormatter.Format(currentSkill.PercToNextLevel) : "0");
                    return true;
                case "skillpercentshouldshow":
                    value = ((currentSkill != null) ? (currentSkill.ProgressionClass.Type == ProgressionType.Skill).ToString() : "false");
                    return true;
                case "grouptype":
                    value = GetGroupType();
                    return true;
                case "grouptype_key":
                    value = GetGroupTypeTextKey();
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        public override bool ParseAttribute(string name, string value, XUiController _parent)
        {
            switch (name)
            {
                case "enabled_color":
                    enabledColor = value;
                    return true;
                case "disabled_color":
                    disabledColor = value;
                    return true;
                case "row_color":
                    rowColor = value;
                    return true;
                case "hover_color":
                    hoverColor = value;
                    return true;
                case "selected_color":
                    selectedColor = value;
                    return true;
                default:
                    return base.ParseAttribute(name, value, _parent);
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            XUiEventManager.Instance.OnSkillExperienceAdded += Current_OnSkillExperienceAdded;
        }

        public override void OnClose()
        {
            base.OnClose();
            XUiEventManager.Instance.OnSkillExperienceAdded -= Current_OnSkillExperienceAdded;
        }

        protected override void OnHovered(bool _isOver)
        {
            base.OnHovered(_isOver);
            if (currentSkill != null && (currentSkill.ProgressionClass.Type != ProgressionType.Skill || IsBook))
            {
                if (IsHovered != _isOver)
                {
                    IsHovered = _isOver;
                    RefreshBindings();
                }
            }
            else
            {
                IsHovered = false;
            }
        }

        private void Current_OnSkillExperienceAdded(ProgressionValue changedSkill, int newXP)
        {
            if (currentSkill == changedSkill)
            {
                RefreshBindings();
            }
        }

        private string GetGroupLevel()
        {
            if (!isBook && currentSkill != null && currentSkill.ProgressionClass.Type != ProgressionType.Skill)
            {
                return groupLevelFormatter.Format(currentSkill.Level, currentSkill.CalculatedLevel(xui.playerUI.entityPlayer), currentSkill.ProgressionClass.MaxLevel);
            }

            return "";
        }

        private string GetGroupPointCost()
        {
            if (currentSkill != null)
            {
                if (!currentSkill.ProgressionClass.IsBook)
                {
                    if (currentSkill.ProgressionClass.CurrencyType != ProgressionCurrencyType.SP)
                    {
                        return "";
                    }

                    if (currentSkill.CostForNextLevel <= 0)
                    {
                        return "NA";
                    }

                    return groupPointCostFormatter.Format(currentSkill.CostForNextLevel);
                }

                int num = 0;
                int num2 = 0;
                for (int i = 0; i < currentSkill.ProgressionClass.Children.Count; i++)
                {
                    num++;
                    if (xui.playerUI.entityPlayer.Progression.GetProgressionValue(currentSkill.ProgressionClass.Children[i].Name).Level == 1)
                    {
                        num2++;
                    }
                }

                num2 = Mathf.Min(num2, num - 1);
                return groupLevelFormatter.Format(num2, num2, num - 1);
            }

            return "";
        }

        private string GetGroupIcon()
        {
            if(currentSkill == null)
            {
                return "";
            }

            if(string.IsNullOrEmpty(currentSkill.ProgressionClass.Icon))
            {
                return "ui_game_filled_circle";
            }

            return currentSkill.ProgressionClass.Icon;
        }

        private string GetGroupTypeIcon()
        {
            if (!isBook || currentSkill == null)
            {
                return "";
            }

            if(currentSkill.ProgressionClass.IsPerk)
            {
                return "ui_game_symbol_perk";
            } 
            
            if(currentSkill.ProgressionClass.IsSkill)
            {
                return "ui_game_symbol_skills";
            } 
            
            if(currentSkill.ProgressionClass.IsAttribute)
            {
                return "ui_game_symbol_hammer";
            } 
            
            return "ui_game_symbol_skills";

        }

        private string GetGroupType()
        {
            if (currentSkill == null)
            {
                return "";
            }

            if (currentSkill.ProgressionClass.IsAttribute)
            {
                return "Attribute";
            }

            if (currentSkill.ProgressionClass.IsPerk)
            {
                return "Perk";
            }

            if (currentSkill.ProgressionClass.IsBook)
            {
                return "Book";
            }

            if (currentSkill.ProgressionClass.IsSkill)
            {
                return "Skill";
            }

            return "";

        }

        private string GetGroupTypeTextKey()
        {
            if (currentSkill == null)
            {
                return "";
            }

            if (currentSkill.ProgressionClass.IsAttribute)
            {
                return "attAttributeTitle";
            }

            if (currentSkill.ProgressionClass.IsPerk)
            {
                return "xuiPerk";
            }

            if (currentSkill.ProgressionClass.IsBook)
            {
                return "xuiBook";
            }

            if (currentSkill.ProgressionClass.IsSkill)
            {
                return "xuiSkill";
            }

            return "";

        }

        private bool CanPurchaseSkill()
        {
            if (currentSkill == null || currentSkill.ProgressionClass.Type == ProgressionType.Skill)
            {
                return false;
            }
            return currentSkill.CanPurchase(xui.playerUI.entityPlayer, currentSkill.Level + 1);
        }
    }
}
