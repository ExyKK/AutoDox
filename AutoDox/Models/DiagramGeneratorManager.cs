using AutoDox.DiagramGenerator.Library;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoDox.UI.Models
{
    class DiagramGeneratorManager
    {
        private Dictionary<string, object> _parameters;
        private HttpRequestManager _requestManager;
        private string _sourcePath;
        private List<string> _pumlFiles;
        private string _pumlPath;

        private static string _logs;
        public static event EventHandler LogsChanged;
        public static string Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                LogsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public async Task<bool> Run()
        {
            _parameters = ConfigurationManager.GetConfiguration();
            _requestManager = new();

            if (_parameters["InputMode"].ToString() == "Select_folder")
            {
                _sourcePath = ExplorerDialog.SelectFolder();
                if (_sourcePath != null)
                {
                    Logs += $"Generation from {_sourcePath} started.\n";
                    if (GeneratePlantUmlFromDir())
                    {
                        await _requestManager.GetSvgFromPlantUml(_pumlFiles);
                        return true;
                    }
                    Logs += "Jobs finished with error.\n";
                }                
            }
            else if (_parameters["InputMode"].ToString() == "Select_file")
            {
                _sourcePath = ExplorerDialog.SelectFile();
                if (_sourcePath != null)
                {
                    Logs += $"Generation from {_sourcePath} started.\n";
                    if (GeneratePlantUmlFromFile())
                    {
                        await _requestManager.GetSvgFromPlantUml(_pumlPath);
                        return true;
                    }
                    Logs += "Jobs finished with error.\n";
                }                
            }
            return false;
        }

        public static string ReadPlantUml(string path)
        {
            StringBuilder pumlString = new();
            using (StreamReader reader = new(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    pumlString.AppendLine(line);
                }
            }
            return pumlString.ToString();
        }

        public static void WriteSvg(string input, string path)
        {
            string destinationDirectory = Path.GetDirectoryName(path);
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            using (StreamWriter writer = new(path))
            {
                writer.Write(input);
            }
        }

        private bool GeneratePlantUmlFromFile()
        {
            if (!File.Exists(_sourcePath))
            {
                Logs += $"\"{_sourcePath}\" does not exist.\n";
                return false;
            }
            _pumlPath = CombinePath(Path.GetFullPath(_parameters["DestinationDirectory"].ToString()),
                                   Path.GetFileNameWithoutExtension(_sourcePath) + ".puml");
            try
            {
                using var stream = new FileStream(_sourcePath, FileMode.Open, FileAccess.Read);
                var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream));
                var root = tree.GetRoot();
                Accessibilities ignoreAcc = GetIgnoreAccessibilities();

                using var filestream = new FileStream(_pumlPath, FileMode.Create, FileAccess.Write);
                using var writer = new StreamWriter(filestream);
                var gen = new ClassDiagramGenerator(
                    writer,
                    "    ",
                    ignoreAcc,
                    (bool)_parameters["AssociationsParameter"],
                    (bool)_parameters["AttributeParameter"],
                    (bool)_parameters["ExcludeTagsParameter"]);
                gen.Generate(root);
            }
            catch (Exception e)
            {
                Logs += $"{e}\n";
                return false;
            }
            return true;
        }

        private bool GeneratePlantUmlFromDir()
        {
            if (!Directory.Exists(_sourcePath))
            {
                Logs += $"Directory \"{_sourcePath}\" does not exist.\n";
                return false;
            }

            var outputRoot = Path.GetFullPath(_sourcePath);
            if (_parameters.TryGetValue("DestinationDirectory", out object outValue))
            {
                outputRoot = outValue.ToString();
                try
                {
                    Directory.CreateDirectory(outputRoot);
                }
                catch (Exception e)
                {
                    Logs += $"{e}\n";
                    return false;
                }
            }

            var excludePaths = new List<string>();
            var pumlexclude = CombinePath(_sourcePath, ".pumlexclude");
            if (File.Exists(pumlexclude))
            {
                excludePaths = File
                    .ReadAllLines(pumlexclude)
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .Select(path => path.Trim())
                    .ToList();
            }
            if (_parameters.TryGetValue("ExcludedPaths", out object excludePathValue))
            {
                var splitOptions = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
                excludePaths.AddRange(excludePathValue.ToString().Split(',', splitOptions));
            }

            var excludeUmlBeginEndTags = (bool)_parameters["ExcludeTagsParameter"];
            var files = Directory.EnumerateFiles(_sourcePath, "*.cs", SearchOption.AllDirectories);

            var includeRefs = new StringBuilder();
            if (!excludeUmlBeginEndTags) includeRefs.AppendLine("@startuml");

            _pumlFiles = new();
            var error = false;
            var filesToProcess = ExcludeFileFilter.GetFilesToProcess(files, excludePaths, _sourcePath);
            foreach (var inputFile in filesToProcess)
            {
                Logs += $"Processing \"{inputFile}\"...\n";
                try
                {
                    var outputDir = CombinePath(outputRoot, Path.GetDirectoryName(inputFile).Replace(_sourcePath, ""));
                    Directory.CreateDirectory(outputDir);
                    var outputFile = CombinePath(outputDir,
                        Path.GetFileNameWithoutExtension(inputFile) + ".puml");

                    using (var stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    {
                        var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream));
                        var root = tree.GetRoot();
                        Accessibilities ignoreAcc = GetIgnoreAccessibilities();

                        using var filestream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                        using var writer = new StreamWriter(filestream);
                        var gen = new ClassDiagramGenerator(
                            writer,
                            "    ",
                            ignoreAcc,
                            (bool)_parameters["AssociationsParameter"],
                            (bool)_parameters["AttributeParameter"],
                            excludeUmlBeginEndTags);
                        gen.Generate(root);
                    }

                    if ((bool)_parameters["AllInOneParameter"])
                    {
                        var lines = File.ReadAllLines(outputFile);
                        if (!excludeUmlBeginEndTags)
                        {
                            lines = lines.Skip(1).SkipLast(1).ToArray();
                        }
                        foreach (string line in lines)
                        {
                            includeRefs.AppendLine(line);
                        }
                    }
                    else
                    {
                        var newRoot = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @".\" : @".";
                        includeRefs.AppendLine("!include " + outputFile.Replace(outputRoot, newRoot));
                    }

                    _pumlFiles.Add(outputFile);
                }
                catch (Exception e)
                {
                    Logs += $"{e}\n";
                    error = true;
                }
            }
            if (!excludeUmlBeginEndTags) includeRefs.AppendLine("@enduml");
            string includeFile = CombinePath(outputRoot, "include.puml");
            File.WriteAllText(includeFile, includeRefs.ToString());

            _pumlFiles.Add(includeFile);

            if (error)
            {
                Logs += "There were files that could not be processed.\n";
                return false;
            }
            return true;
        }

        private Accessibilities GetIgnoreAccessibilities()
        {
            var ignoreAcc = Accessibilities.None;
            if ((bool)_parameters["PublicMembersParameter"])
            {
                ignoreAcc = Accessibilities.Private | Accessibilities.Internal
                    | Accessibilities.Protected | Accessibilities.ProtectedInternal;
            }
            else if (_parameters.TryGetValue("SelectedModifierItems", out object value))
            {
                var ignoreItems = value.ToString().Split(',');
                foreach (var item in ignoreItems)
                {
                    if (Enum.TryParse(item, true, out Accessibilities acc))
                    {
                        ignoreAcc |= acc;
                    }
                }
            }
            return ignoreAcc;
        }

        private static string CombinePath(string first, string second)
        {
            return PathHelper.CombinePath(first, second);
        }
    }
}
