using AutoDox.DiagramGenerator.Library;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoDox.UI.Models
{
    class DiagramGeneratorManager
    {
        private Dictionary<string, object> parameters;
        private HttpRequestManager requestManager;
        private string sourcePath;
        private List<string> pumlFiles;
        private string pumlPath;

        public void Run()
        {
            parameters = ConfigurationManager.GetConfiguration();
            requestManager = new();                

            if (parameters["InputMode"].ToString() == "Select_folder")
            {
                sourcePath = ExplorerDialog.SelectFolder();
                if (sourcePath != null)
                {
                    if(GeneratePlantUmlFromDir())
                    {
                        requestManager.GetSvgFromPlantUml(pumlFiles);
                    }
                    else
                    {
                        // handle error
                    }
                }
            }
            else if (parameters["InputMode"].ToString() == "Select_file")
            {
                sourcePath = ExplorerDialog.SelectFile();
                if (sourcePath != null)
                {
                    if (GeneratePlantUmlFromFile())
                    {
                        requestManager.GetSvgFromPlantUml(pumlPath);
                    }
                    else
                    {
                        // handle error
                    }
                }
            }
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
            if (!File.Exists(sourcePath))
            {
                Console.WriteLine($"\"{sourcePath}\" does not exist.");
                return false;
            }
            pumlPath = CombinePath(Path.GetFullPath(parameters["DestinationDirectory"].ToString()),
                                   Path.GetFileNameWithoutExtension(sourcePath) + ".puml");
            try
            {
                using var stream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream));
                var root = tree.GetRoot();
                Accessibilities ignoreAcc = GetIgnoreAccessibilities();

                using var filestream = new FileStream(pumlPath, FileMode.Create, FileAccess.Write);
                using var writer = new StreamWriter(filestream);
                var gen = new ClassDiagramGenerator(
                    writer,
                    "    ",
                    ignoreAcc,
                    (bool)parameters["AssociationsParameter"],
                    (bool)parameters["AttributeParameter"],
                    (bool)parameters["ExcludeTagsParameter"]);
                gen.Generate(root);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        private bool GeneratePlantUmlFromDir()
        {
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"Directory \"{sourcePath}\" does not exist.");
                return false;
            }

            var outputRoot = Path.GetFullPath(sourcePath);
            if (parameters.TryGetValue("DestinationDirectory", out object outValue))
            {
                outputRoot = outValue.ToString();
                try
                {
                    Directory.CreateDirectory(outputRoot);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }

            var excludePaths = new List<string>();
            var pumlexclude = CombinePath(sourcePath, ".pumlexclude");
            if (File.Exists(pumlexclude))
            {
                excludePaths = File
                    .ReadAllLines(pumlexclude)
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .Select(path => path.Trim())
                    .ToList();
            }
            if (parameters.TryGetValue("ExcludedPaths", out object excludePathValue))
            {
                var splitOptions = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
                excludePaths.AddRange(excludePathValue.ToString().Split(',', splitOptions));
            }

            var excludeUmlBeginEndTags = (bool)parameters["ExcludeTagsParameter"];
            var files = Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories);

            var includeRefs = new StringBuilder();
            if (!excludeUmlBeginEndTags) includeRefs.AppendLine("@startuml");

            pumlFiles = new();
            var error = false;
            var filesToProcess = ExcludeFileFilter.GetFilesToProcess(files, excludePaths, sourcePath);
            foreach (var inputFile in filesToProcess)
            {
                Console.WriteLine($"Processing \"{inputFile}\"...");
                try
                {
                    var outputDir = CombinePath(outputRoot, Path.GetDirectoryName(inputFile).Replace(sourcePath, ""));
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
                            (bool)parameters["AssociationsParameter"],
                            (bool)parameters["AttributeParameter"],
                            excludeUmlBeginEndTags);
                        gen.Generate(root);
                    }

                    if ((bool)parameters["AllInOneParameter"])
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

                    pumlFiles.Add(outputFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    error = true;
                }
            }
            if (!excludeUmlBeginEndTags) includeRefs.AppendLine("@enduml");
            string includeFile = CombinePath(outputRoot, "include.puml");
            File.WriteAllText(includeFile, includeRefs.ToString());

            pumlFiles.Add(includeFile);

            if (error)
            {
                Console.WriteLine("There were files that could not be processed.");
                return false;
            }
            return true;
        }

        private Accessibilities GetIgnoreAccessibilities()
        {
            var ignoreAcc = Accessibilities.None;
            if ((bool)parameters["PublicMembersParameter"])
            {
                ignoreAcc = Accessibilities.Private | Accessibilities.Internal
                    | Accessibilities.Protected | Accessibilities.ProtectedInternal;
            }
            else if (parameters.TryGetValue("SelectedModifierItems", out object value))
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
