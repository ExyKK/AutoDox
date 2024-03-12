using AutoDox.UI.ViewModels;
using HandyControl.Themes;

namespace AutoDox.UI.Core
{
    internal class ThemeChanger
    {
        public static void Change()
        {
            if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                HomeViewModel.SvgColor = "#FFFFFF";
                SettingsViewModel.Theme = GetTheme();
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                HomeViewModel.SvgColor = "#000000";
                SettingsViewModel.Theme = GetTheme();
            }
        }

        public static string GetTheme()
        {
            return ThemeManager.Current.ApplicationTheme.ToString();
        }

        public static string GetSvgColor()
        {
            return (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light) ? "#000000" : "#FFFFFF";
        }
    }
}
