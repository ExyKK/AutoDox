using System.IO;

namespace AutoDox.UI.Models
{
    internal static class PathHelper
    {
        public static string CombinePath(string first, string second)
        {
            return first.TrimEnd(Path.DirectorySeparatorChar)
                   + Path.DirectorySeparatorChar
                   + second.TrimStart(Path.DirectorySeparatorChar);
        }
    }
}
