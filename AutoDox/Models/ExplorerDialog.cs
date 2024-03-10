using Microsoft.Win32;

namespace AutoDox.UI.Models
{
    class ExplorerDialog
    {
        public string SelectFolder()
        {
            OpenFolderDialog dialog = new();

            if ((bool)dialog.ShowDialog())
            {
               return dialog.FolderName;
            }
            return null;
        }

        public string[] SelectFiles()
        {
            OpenFileDialog dialog = new() { Multiselect = true };

            if ((bool)dialog.ShowDialog())
            {
                return dialog.FileNames;
            }
            return null;
        }
    }
}
