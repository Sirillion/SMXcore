using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SMXcore
{
    public class XUiC_SkillBookLevel : global::XUiC_SkillBookLevel
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
            if (Perk == null || Perk.ProgressionClass == null)
            {
                return "";
            }

            if(!string.IsNullOrEmpty(Perk.ProgressionClass.LongDescKey))
            {
                return Localization.Get(Perk.ProgressionClass.LongDescKey);
            }

            return Localization.Get(Perk.ProgressionClass.DescKey);
        }
    }
}
