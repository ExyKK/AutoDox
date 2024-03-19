using Microsoft.Win32;

namespace AutoDox.UI.Models
{
    class ExplorerDialog
    {
        public static string SelectFolder()
        {
            OpenFolderDialog dialog = new();

            if ((bool)dialog.ShowDialog())
            {
               return dialog.FolderName;
            }
            return null;
        }

        public static string SelectFile()
        {
            OpenFileDialog dialog = new();

            if ((bool)dialog.ShowDialog())
            {
                return dialog.FileName;
            }
            return null;
        }
    }
}
