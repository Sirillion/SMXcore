using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_SkillPerkLevel : global::XUiC_SkillPerkLevel
    {
        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "longdescription":
                    value = GetLongDescription();
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);
            }
        }

        private string GetLongDescription()
        {
            if (CurrentSkill == null
                || CurrentSkill.ProgressionClass == null
                || Level > CurrentSkill.ProgressionClass.MaxLevel
                || CurrentSkill.ProgressionClass.Effects == null
                || CurrentSkill.ProgressionClass.Effects.EffectGroups == null)
            {
                return "";
            }

            foreach (MinEffectGroup minEffectGroup in CurrentSkill.ProgressionClass.Effects.EffectGroups)
            {
                if (minEffectGroup.EffectDescriptions != null)
                {
                    for (int i = 0; i < minEffectGroup.EffectDescriptions.Count; i++)
                    {
                        if (Level >= minEffectGroup.EffectDescriptions[i].MinLevel
                            && Level <= minEffectGroup.EffectDescriptions[i].MaxLevel)
                        {
                            return !string.IsNullOrEmpty(minEffectGroup.EffectDescriptions[i].LongDescription)
                                ? minEffectGroup.EffectDescriptions[i].LongDescription
                                : minEffectGroup.EffectDescriptions[i].Description;
                        }
                    }
                }
            }

            return "";
        }
    }
}
