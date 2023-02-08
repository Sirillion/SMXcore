
//	Terms of Use: You can use this file as you want as long as this line and the credit lines are not removed or changed other than adding to them!
//	Credits: The Fun Pimps.
//	Tweaked: Laydor

//	Changes CategoryEntry to be able to be placed on a non XUiV_Button

namespace SMXcore
{
    public class XUiC_SkillCategoryEntry : XUiController
    {
        private string categoryName = "";

        private string categoryDisplayName = "";

        private string spriteName = "";

        private bool selected;

        private XUiV_Button button;

        public XUiC_SkillCategoryList CategoryList { get; set; }

        public string CategoryName
        {
            get
            {
                return categoryName;
            }
            set
            {
                categoryName = value;
                IsDirty = true;
            }
        }

        public string CategoryDisplayName
        {
            get
            {
                return categoryDisplayName;
            }
            set
            {
                categoryDisplayName = value;
                IsDirty = true;
            }
        }

        public string SpriteName
        {
            get
            {
                return spriteName;
            }
            set
            {
                spriteName = value;
                IsDirty = true;
            }
        }

        public new bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                button.Selected = selected;
            }
        }

        public override void Init()
        {
            base.Init();
            XUiController buttonController = GetChildById("button");
            if(buttonController != null)
            {
                button = (XUiV_Button)buttonController.ViewComponent;
                buttonController.OnPress += XUiC_CategoryEntry_OnPress;
            }
            IsDirty = true;
        }

        private void XUiC_CategoryEntry_OnPress(XUiController _sender, int _mouseButton)
        {
            if (spriteName != string.Empty)
            {
                if (CategoryList.CurrentCategory == this && CategoryList.AllowUnselect)
                {
                    CategoryList.CurrentCategory = null;
                }
                else
                {
                    CategoryList.CurrentCategory = this;
                }

                CategoryList.HandleCategoryChanged();
            }
        }

        public override void Update(float _dt)
        {
            button.shouldSnap = !string.IsNullOrEmpty(categoryName);
            base.Update(_dt);
            if(IsDirty)
            {
                RefreshBindings();
                IsDirty = false;
            }
        }

        public override bool GetBindingValue(ref string value, string bindingName)
        {
            switch (bindingName)
            {
                case "categoryicon":
                    value = spriteName;
                    return true;
                case "categorydisplayname":
                    value = categoryDisplayName;
                    return true;
                default:
                    return false;
            }
        }
    }
}
