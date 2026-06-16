using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PatchProcessing
{
    public static class SimpleYamlParser
    {
        public static List<Patch> Parse(string yaml)
        {
            var patches = new List<Patch>();
            if (string.IsNullOrWhiteSpace(yaml))
                return patches;

            // Нормализуем переносы строк на \n
            yaml = yaml.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = yaml.Split('\n');
            int i = 0;
            while (i < lines.Length)
            {
                string trimmed = lines[i].TrimStart();
                if (trimmed.StartsWith("- action:"))
                {
                    string actionValue = trimmed.Substring("- action:".Length).Trim();
                    var patch = new Patch { action = actionValue, arguments = null };
                    i++;

                    while (i < lines.Length && (lines[i].StartsWith("  ") || string.IsNullOrWhiteSpace(lines[i])))
                    {
                        string argLine = lines[i];
                        if (string.IsNullOrWhiteSpace(argLine))
                        {
                            i++;
                            continue;
                        }

                        int colonIndex = argLine.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            string key = argLine.Substring(0, colonIndex).Trim();
                            string valuePart = argLine.Substring(colonIndex + 1).TrimStart();

                            if (patch.arguments == null)
                                patch.arguments = new Arguments();

                            if (valuePart == "|" || valuePart == "|-" || valuePart == "|+")
                            {
                                i++;
                                var blockLines = new List<string>();
                                while (i < lines.Length && (lines[i].StartsWith("    ") || lines[i].Trim() == ""))
                                {
                                    blockLines.Add(lines[i]);
                                    i++;
                                }
                                int minIndent = int.MaxValue;
                                foreach (var line in blockLines)
                                {
                                    if (line.Trim() == "")
                                        continue;
                                    int indent = line.Length - line.TrimStart().Length;
                                    if (indent < minIndent)
                                        minIndent = indent;
                                }
                                if (minIndent == int.MaxValue)
                                    minIndent = 0;
                                var sb = new StringBuilder();
                                for (int j = 0; j < blockLines.Count; j++)
                                {
                                    string blockLine = blockLines[j];
                                    if (blockLine.Trim() == "")
                                    {
                                        sb.AppendLine();
                                    }
                                    else
                                    {
                                        string contentLine = blockLine.Substring(minIndent);
                                        sb.AppendLine(contentLine);
                                    }
                                }
                                string blockValue = sb.ToString().TrimEnd('\r', '\n');
                                SetValue(patch.arguments, key, blockValue);
                                i--;
                            }
                            else
                            {
                                string singleValue = valuePart.Trim();
                                if (singleValue.StartsWith("\"") && singleValue.EndsWith("\""))
                                    singleValue = singleValue.Substring(1, singleValue.Length - 2);
                                SetValue(patch.arguments, key, singleValue);
                            }
                        }
                        i++;
                    }
                    patches.Add(patch);
                }
                else
                {
                    i++;
                }
            }
            return patches;
        }

        private static void SetValue(Arguments args, string key, string value)
        {
            switch (key)
            {
                case "file_path":
                    args.file_path = value;
                    break;
                case "file_content":
                    args.file_content = value;
                    break;
                case "search":
                    args.search = value;
                    break;
                case "replace":
                    args.replace = value;
                    break;
                case "occurrence":
                    if (int.TryParse(value, out _))
                        args.occurrence = int.Parse(value);
                    else
                        args.occurrence = value;
                    break;
                case "directory":
                    args.directory = value;
                    break;
                case "source_path":
                    args.source_path = value;
                    break;
                case "destination_path":
                    args.destination_path = value;
                    break;
                case "class_name":
                    args.class_name = value;
                    break;
                case "search_line_contains":
                    args.search_line_contains = value;
                    break;
            }
        }
    }
}
