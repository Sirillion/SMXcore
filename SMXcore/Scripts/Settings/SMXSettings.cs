using GearsAPI.Settings.Global;

public class SMXSettings
{
    public static void SkipNewsScreen(IGlobalModSetting setting, string newValue)
    {
        bool skipNews = newValue == "On";

        XUiC_MainMenu.shownNewsScreenOnce = skipNews;
    }
}
