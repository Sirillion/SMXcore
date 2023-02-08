//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: Laydor.

//	A container controller for SkillSubEntries to set which ones should be visible or not at a given time

namespace SMXcore
{
    public class XUiC_SkillEntry : XUiController
    {
        private XUiC_SkillSubEntry attributeEntry;

        private XUiC_SkillSubEntry skillEntry1;
        private XUiC_SkillSubEntry skillEntry2;
        private XUiC_SkillSubEntry bookEntry1;
        private XUiC_SkillSubEntry bookEntry2;

        private bool isBooks;

        public override void Init()
        {
            base.Init();

            attributeEntry = GetChildById("smxui_attribute_entry") as XUiC_SkillSubEntry;
            skillEntry1 = GetChildById("smxui_perk_entry_1") as XUiC_SkillSubEntry;
            skillEntry2 = GetChildById("smxui_perk_entry_2") as XUiC_SkillSubEntry;

            bookEntry1 = GetChildById("smxui_book_entry_1") as XUiC_SkillSubEntry;
            bookEntry2 = GetChildById("smxui_book_entry_2") as XUiC_SkillSubEntry;
        }

        public void SetSkillEntries(ProgressionValue skill1, ProgressionValue skill2, bool IsBooks)
        {
            attributeEntry.Skill = null;
            attributeEntry.ViewComponent.Enabled = false;

            isBooks = IsBooks;

            if(IsBooks && bookEntry1 != null && bookEntry2 != null)
            {
                skillEntry1.Skill = null;
                skillEntry1.ViewComponent.Enabled = false;

                bookEntry1.Skill = skill1;
                bookEntry1.IsBook = IsBooks;
                bookEntry1.ViewComponent.Enabled = skill1 != null;

                skillEntry2.Skill = null;
                skillEntry2.ViewComponent.Enabled = false;

                bookEntry2.Skill = skill2;
                bookEntry2.IsBook = IsBooks;
                bookEntry2.ViewComponent.Enabled = skill2 != null;

            }
            else
            {
                if (bookEntry1 != null)
                {
                    bookEntry1.Skill = null;
                    bookEntry1.ViewComponent.Enabled = false;
                }

                if (bookEntry2 != null)
                {
                    bookEntry2.Skill = null;
                    bookEntry2.ViewComponent.Enabled = false;
                }

                skillEntry1.Skill = skill1;
                skillEntry1.IsBook = IsBooks;
                skillEntry1.ViewComponent.Enabled = skill1 != null;

                skillEntry2.Skill = skill2;
                skillEntry2.IsBook = IsBooks;
                skillEntry2.ViewComponent.Enabled = skill2 != null;
            }

            RefreshBindings();
        }

        public void SetAttributeEntry(ProgressionValue attribute)
        {
            attributeEntry.Skill = attribute;
            attributeEntry.ViewComponent.Enabled = true;

            skillEntry1.Skill = null;
            skillEntry1.ViewComponent.Enabled = false;

            skillEntry2.Skill = null;
            skillEntry2.ViewComponent.Enabled = false;

            if (bookEntry1 != null)
            {
                bookEntry1.Skill = null;
                bookEntry1.ViewComponent.Enabled = false;
            }

            if (bookEntry2 != null)
            {
                bookEntry2.Skill = null;
                bookEntry2.ViewComponent.Enabled = false;

            }

            RefreshBindings();
        }

        public void ClearEntries()
        {
            attributeEntry.Skill = null;
            attributeEntry.ViewComponent.Enabled = false;

            skillEntry1.Skill = null;
            skillEntry1.ViewComponent.Enabled = false;

            skillEntry2.Skill = null;
            skillEntry2.ViewComponent.Enabled = false;

            if(bookEntry1 != null)
            {
                bookEntry1.Skill = null;
                bookEntry1.ViewComponent.Enabled = false;
            }

            if (bookEntry2 != null)
            {
                bookEntry2.Skill = null;
                bookEntry2.ViewComponent.Enabled = false;
            }

            RefreshBindings();
        }

        public XUiC_SkillSubEntry GetFirstEntry()
        {
            if(attributeEntry.Skill != null)
            {
                return attributeEntry;
            }

            return isBooks && bookEntry1 != null ? bookEntry1 : skillEntry1;
        }

        public XUiC_SkillSubEntry GetSecondEntry()
        {
            {
                if (attributeEntry.Skill != null)
                {
                    return attributeEntry;
                }

                return isBooks && bookEntry2 != null ? bookEntry2 : skillEntry2;
            }
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "hasentries":
                    value = ((attributeEntry != null && attributeEntry.Skill != null) || (skillEntry1 != null && skillEntry1.Skill != null) || (bookEntry1 != null && bookEntry1.Skill != null)).ToString();
                    return true;
                default:
                    return base.GetBindingValue(ref value, bindingName);

            }
        }
    }
}
