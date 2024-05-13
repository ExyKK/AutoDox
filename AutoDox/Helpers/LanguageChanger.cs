using System.Globalization;
using System.Threading;
using System.Windows;

namespace AutoDox.UI.Helpers
{
    internal class LanguageChanger
    {
        public static void Change()
        {
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Application.Current.MainWindow.UpdateLayout();
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                Application.Current.MainWindow.UpdateLayout();
            }
        }

        public static string GetLanguage()
        {
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru")
            {
                return "Русский";
            }
            else
            {
                return "English";
            }
        }
    }
}
