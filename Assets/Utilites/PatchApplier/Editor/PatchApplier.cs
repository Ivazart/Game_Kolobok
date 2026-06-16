using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PatchProcessing
{
    public class PatchApplier : EditorWindow
    {
        // ---------------------------------------------------------
        // Глобальные настройки (сохраняются в EditorPrefs)
        // ---------------------------------------------------------
        
        private const string PrefKey_Allowed = "PatchApplier_AllowedPrefixes";
        private const string PrefKey_Blocked = "PatchApplier_BlockedPrefixes";
        private const string PrefKey_JsonPath = "PatchApplier_DefaultJsonPath";
        private const string PrefKey_YamlPath = "PatchApplier_DefaultYamlPath";

        /// <summary>Вызывать AssetDatabase.Refresh() автоматически после каждого действия.</summary>
        public static bool AutoRefresh = true;

        /// <summary>Белый список префиксов путей (относительно Assets).</summary>
        public static List<string> AllowedPathPrefixes = new List<string>();
        
        /// <summary>Чёрный список префиксов путей (относительно Assets).</summary>
        public static List<string> BlockedPathPrefixes = new List<string>
        {
            "Plugins/",
            "Resources/",
            "StreamingAssets/",
            "Standard Assets/"
        };

        /// <summary>Пользовательские пути к дефолтным патчам (null = автоопределение).</summary>
        public static string CustomDefaultJsonPath = null;
        public static string CustomDefaultYamlPath = null;

        static PatchApplier()
        {
            LoadSettings();
        }

        private static void LoadSettings()
        {
            // Загружаем списки
            string allowedJson = EditorPrefs.GetString(PrefKey_Allowed, "");
            if (!string.IsNullOrEmpty(allowedJson))
            {
                try { AllowedPathPrefixes = JsonConvert.DeserializeObject<List<string>>(allowedJson); }
                catch { /* оставляем дефолтный пустой список */ }
            }

            string blockedJson = EditorPrefs.GetString(PrefKey_Blocked, "");
            if (!string.IsNullOrEmpty(blockedJson))
            {
                try { BlockedPathPrefixes = JsonConvert.DeserializeObject<List<string>>(blockedJson); }
                catch { /* остаётся дефолтный */ }
            }

            // Загружаем пути к патчам (если сохранены)
            string jsonPath = EditorPrefs.GetString(PrefKey_JsonPath, "");
            if (!string.IsNullOrEmpty(jsonPath)) CustomDefaultJsonPath = jsonPath;

            string yamlPath = EditorPrefs.GetString(PrefKey_YamlPath, "");
            if (!string.IsNullOrEmpty(yamlPath)) CustomDefaultYamlPath = yamlPath;
        }

        public static void SaveSettings()
        {
            EditorPrefs.SetString(PrefKey_Allowed, JsonConvert.SerializeObject(AllowedPathPrefixes));
            EditorPrefs.SetString(PrefKey_Blocked, JsonConvert.SerializeObject(BlockedPathPrefixes));
            EditorPrefs.SetString(PrefKey_JsonPath, CustomDefaultJsonPath ?? "");
            EditorPrefs.SetString(PrefKey_YamlPath, CustomDefaultYamlPath ?? "");
            Debug.Log("PatchApplier settings saved.");
        }

        public static void ResetToDefaults()
        {
            // Очищаем EditorPrefs
            EditorPrefs.DeleteKey(PrefKey_Allowed);
            EditorPrefs.DeleteKey(PrefKey_Blocked);
            EditorPrefs.DeleteKey(PrefKey_JsonPath);
            EditorPrefs.DeleteKey(PrefKey_YamlPath);

            // Восстанавливаем дефолтные значения
            AllowedPathPrefixes = new List<string>();
            BlockedPathPrefixes = new List<string>
            {
                "Plugins/",
                "Resources/",
                "StreamingAssets/",
                "Standard Assets/"
            };
            CustomDefaultJsonPath = null;
            CustomDefaultYamlPath = null;
            Debug.Log("PatchApplier settings reset to defaults.");
        }

        // ---------------------------------------------------------
        // Вспомогательные методы для путей
        // ---------------------------------------------------------
        private static string GetScriptDirectory()
        {
            string[] guids = AssetDatabase.FindAssets("t:Script PatchApplier");
            if (guids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                return Path.GetDirectoryName(scriptPath);
            }
            return null;
        }

        private static string GetDefaultJsonPath()
        {
            if (!string.IsNullOrWhiteSpace(CustomDefaultJsonPath) && File.Exists(CustomDefaultJsonPath))
                return CustomDefaultJsonPath;
            string scriptDir = GetScriptDirectory();
            if (scriptDir == null) return null;
            // Ищем в родительской папке (PatchApplier/), чтобы не смешивать с кодом
            string parentDir = Directory.GetParent(scriptDir)?.FullName;
            if (parentDir != null)
            {
                string path = Path.Combine(parentDir, "patch.json");
                if (File.Exists(path))
                    return path;
            }
            // Запасной вариант: рядом со скриптом (Editor/)
            string fallback = Path.Combine(scriptDir, "patch.json");
            return File.Exists(fallback) ? fallback : null;
        }

        private static string GetDefaultYamlPath()
        {
            if (!string.IsNullOrWhiteSpace(CustomDefaultYamlPath) && File.Exists(CustomDefaultYamlPath))
                return CustomDefaultYamlPath;
            string scriptDir = GetScriptDirectory();
            if (scriptDir == null) return null;
            // Сначала ищем в родительской папке (PatchApplier/)
            string parentDir = Directory.GetParent(scriptDir)?.FullName;
            if (parentDir != null)
            {
                foreach (var candidate in new[] { "patch.yaml", "patch.yml" })
                {
                    string path = Path.Combine(parentDir, candidate);
                    if (File.Exists(path))
                        return path;
                }
            }
            // Запасной вариант: рядом со скриптом (Editor/)
            foreach (var candidate in new[] { "patch.yaml", "patch.yml" })
            {
                string path = Path.Combine(scriptDir, candidate);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        // ---------------------------------------------------------
        // Меню
        // ---------------------------------------------------------
        [MenuItem("Tools/PatchApplier/Apply from File...")]
        public static void ApplyPatchesFromMenu()
        {
            string assetsRoot = Application.dataPath;
            string filePath = EditorUtility.OpenFilePanel(
                "Select patch file", assetsRoot, "json,yaml,yml");

            if (string.IsNullOrEmpty(filePath))
                return;

            ApplyPatchesFromFile(filePath);
        }

        [MenuItem("Tools/PatchApplier/Apply Default JSON Patch")]
        public static void ApplyDefaultJsonPatch()
        {
            string filePath = GetDefaultJsonPath();
            if (filePath == null)
            {
                Debug.LogWarning("Default JSON patch not found. Set custom path in Safety Settings or place patch.json next to script.");
                return;
            }
            ApplyPatchesFromFile(filePath);
        }

        [MenuItem("Tools/PatchApplier/Apply Default YAML Patch")]
        public static void ApplyDefaultYamlPatch()
        {
            string filePath = GetDefaultYamlPath();
            if (filePath == null)
            {
                Debug.LogWarning("Default YAML patch not found. Set custom path in Safety Settings or place patch.yaml next to script.");
                return;
            }
            ApplyPatchesFromFile(filePath);
        }

        [MenuItem("Tools/PatchApplier/Safety Settings...")]
        public static void OpenSafetySettings()
        {
            var window = GetWindow<SafetySettingsWindow>("Patch Safety Settings");
            window.Show();
        }

        // ---------------------------------------------------------
        // Применение патчей
        // ---------------------------------------------------------
        public static void ApplyPatchesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Patch file not found: {filePath}");
                return;
            }

            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string content = File.ReadAllText(filePath);

            List<Patch> patches = null;
            try
            {
                if (extension == ".yaml" || extension == ".yml")
                    patches = SimpleYamlParser.Parse(content);
                else
                    patches = JsonConvert.DeserializeObject<List<Patch>>(content);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse patch file: {e.Message}");
                return;
            }

            if (patches == null || patches.Count == 0)
            {
                Debug.LogWarning("No patches found in file.");
                return;
            }

            ApplyPatches(patches);
        }

        public static void ApplyPatches(List<Patch> patches)
        {
            string workingDir = Application.dataPath;

            for (int i = 0; i < patches.Count; i++)
            {
                Patch patch = patches[i];
                if (patch == null || (string.IsNullOrEmpty(patch.action) && patch.arguments == null))
                {
                    if (!string.IsNullOrEmpty(patch?._comment))
                        continue;
                    Debug.LogWarning($"Patch #{i + 1}: skipped (invalid format)");
                    continue;
                }
                if (string.IsNullOrEmpty(patch.action))
                {
                    Debug.LogWarning($"Patch #{i + 1}: skipped (missing action)");
                    continue;
                }

                try
                {
                    string result = ExecuteAction(patch, workingDir);
                    Debug.Log($"[{i + 1}] {patch.action}: {result}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[{i + 1}] {patch.action}: ERROR - {e.Message}");
                }
            }
            if (AutoRefresh)
            {
                AssetDatabase.Refresh();
            }
        }

        // ---------------------------------------------------------
        // Выполнение действий
        // ---------------------------------------------------------
        private static string ExecuteAction(Patch patch, string baseDir)
        {
            var args = patch.arguments;
            if (args == null)
                throw new ArgumentException("Missing arguments");

            switch (patch.action)
            {
                case "read_file":
                    return ReadFile(args.file_path, baseDir);
                case "create_file":
                    return CreateFile(args.file_path, args.file_content, baseDir);
                case "overwrite_file":
                    return OverwriteFile(args.file_path, args.file_content, baseDir);
                case "delete_file":
                    return DeleteFile(args.file_path, baseDir);
                case "replace_in_file":
                    return ReplaceInFile(args.file_path, args.search, args.replace,
                        args.occurrence?.ToString() ?? "1", baseDir);
                case "list_files":
                    return ListFiles(args.directory ?? "", baseDir);
                case "move_file":
                    return MoveFile(args.source_path, args.destination_path, baseDir);
                case "file_exists":
                    return FileExists(args.file_path, baseDir);
                case "insert_before_class_end":
                    return InsertBeforeClassEnd(args.file_path, args.class_name, args.file_content, baseDir);
                case "insert_after_line":
                    return InsertAfterLine(args.file_path, args.search_line_contains, args.file_content, baseDir);
                default:
                    throw new NotSupportedException($"Unknown action: {patch.action}");
            }
        }

        // ---------------------------------------------------------
        // Проверка пути
        // ---------------------------------------------------------
        private static bool IsPathAllowed(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            string normalized = relativePath.Replace('\\', '/').TrimStart('/');

            if (AllowedPathPrefixes != null && AllowedPathPrefixes.Count > 0)
            {
                return AllowedPathPrefixes.Any(prefix =>
                {
                    string normalizedPrefix = prefix.Replace('\\', '/').TrimStart('/');
                    return normalized.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase);
                });
            }

            if (BlockedPathPrefixes != null)
            {
                if (BlockedPathPrefixes.Any(prefix =>
                {
                    string normalizedPrefix = prefix.Replace('\\', '/').TrimStart('/');
                    return normalized.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase);
                }))
                {
                    return false;
                }
            }

            return true;
        }

        // ---------------------------------------------------------
        // Существующие файловые операции
        // ---------------------------------------------------------
        private static string ReadFile(string relativePath, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            string fullPath = Path.Combine(baseDir, relativePath);
            if (!File.Exists(fullPath))
                return $"File not found: {fullPath}";
            return File.ReadAllText(fullPath);
        }

        private static string CreateFile(string relativePath, string content, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            string fullPath = Path.Combine(baseDir, relativePath);
            if (File.Exists(fullPath))
                return $"File already exists: {fullPath}. Use overwrite_file or delete first.";
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, content ?? "");
            if (AutoRefresh) AssetDatabase.Refresh();
            return $"File created: {fullPath}";
        }

        private static string OverwriteFile(string relativePath, string content, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            string fullPath = Path.Combine(baseDir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, content ?? "");
            if (AutoRefresh) AssetDatabase.Refresh();
            return $"File overwritten: {fullPath}";
        }

        private static string DeleteFile(string relativePath, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            string fullPath = Path.Combine(baseDir, relativePath);
            if (!File.Exists(fullPath))
                return $"File not found: {fullPath}";
            File.Delete(fullPath);
            string metaPath = fullPath + ".meta";
            if (File.Exists(metaPath))
                File.Delete(metaPath);
            if (AutoRefresh) AssetDatabase.Refresh();
            return $"File deleted: {fullPath}";
        }

        private static string ReplaceInFile(string relativePath, string search, string replace,
            string occurrenceStr, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            string fullPath = Path.Combine(baseDir, relativePath);
            if (!File.Exists(fullPath))
                return $"File not found: {fullPath}";

            string content = File.ReadAllText(fullPath);

            if (occurrenceStr == "all")
            {
                if (!content.Contains(search))
                    return $"String '{search}' not found in {relativePath}";
                content = content.Replace(search, replace);
                File.WriteAllText(fullPath, content);
                if (AutoRefresh) AssetDatabase.Refresh();
                return $"Replaced all occurrences in {relativePath}";
            }

            if (!int.TryParse(occurrenceStr, out int occNum) || occNum < 1)
                return $"Invalid occurrence value: {occurrenceStr}";

            int start = 0, found = 0;
            while (true)
            {
                int idx = content.IndexOf(search, start);
                if (idx == -1) break;
                found++;
                if (found == occNum)
                {
                    content = content.Substring(0, idx) + replace + content.Substring(idx + search.Length);
                    File.WriteAllText(fullPath, content);
                    if (AutoRefresh) AssetDatabase.Refresh();
                    return $"Replaced occurrence #{occNum} in {relativePath}";
                }
                start = idx + search.Length;
            }

            if (found == 0)
                return $"String '{search}' not found in {relativePath}";
            else
                return $"Only {found} occurrences found, cannot replace #{occNum}";
        }

        private static string ListFiles(string directory, string baseDir)
        {
            if (!IsPathAllowed(directory))
                return $"Access denied: '{directory}' is not allowed by safety rules.";
            string fullDir = Path.Combine(baseDir, directory);
            if (!Directory.Exists(fullDir))
                return $"Directory not found: {fullDir}";

            var files = Directory.GetFiles(fullDir, "*.*", SearchOption.AllDirectories)
                                 .Take(100)
                                 .Select(f => Path.GetRelativePath(baseDir, f));
            return string.Join("\n", files);
        }

        private static string MoveFile(string sourceRelative, string destRelative, string baseDir)
        {
            if (!IsPathAllowed(sourceRelative))
                return $"Access denied: source '{sourceRelative}' is not allowed by safety rules.";
            if (!IsPathAllowed(destRelative))
                return $"Access denied: destination '{destRelative}' is not allowed by safety rules.";

            string sourceFull = Path.Combine(baseDir, sourceRelative);
            string destFull = Path.Combine(baseDir, destRelative);

            if (!File.Exists(sourceFull))
                return $"Source file not found: {sourceFull}";

            if (File.Exists(destFull))
                return $"Destination already exists: {destFull}. Delete it first or choose a different name.";

            Directory.CreateDirectory(Path.GetDirectoryName(destFull));
            File.Move(sourceFull, destFull);

            string sourceMeta = sourceFull + ".meta";
            string destMeta = destFull + ".meta";
            if (File.Exists(sourceMeta))
            {
                File.Move(sourceMeta, destMeta);
            }

            if (AutoRefresh) AssetDatabase.Refresh();
            return $"Moved: {sourceFull} -> {destFull}";
        }

        // ---------------------------------------------------------
        // Новые операции
        // ---------------------------------------------------------
        private static string FileExists(string relativePath, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return "Access denied";
            string fullPath = Path.Combine(baseDir, relativePath);
            return File.Exists(fullPath).ToString().ToLower();
        }

        private static string InsertBeforeClassEnd(string relativePath, string className, string code, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            string fullPath = Path.Combine(baseDir, relativePath);
            if (!File.Exists(fullPath))
                return $"File not found: {fullPath}";

            string[] lines = File.ReadAllLines(fullPath);
            int classStartIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Contains("class "))
                {
                    if (string.IsNullOrEmpty(className) || line.Contains(className))
                    {
                        classStartIndex = i;
                        break;
                    }
                }
            }
            if (classStartIndex == -1)
                return $"Class '{className ?? "any"}' not found in {relativePath}";

            int braceBalance = 0;
            int classEndLineIndex = -1;
            for (int i = classStartIndex; i < lines.Length; i++)
            {
                string currentLine = lines[i];
                foreach (char c in currentLine)
                {
                    if (c == '{') braceBalance++;
                    else if (c == '}') braceBalance--;
                }
                if (braceBalance == 0)
                {
                    classEndLineIndex = i;
                    break;
                }
            }
            if (classEndLineIndex == -1)
                return "Could not determine end of class (unbalanced braces)";

            string closeBraceLine = lines[classEndLineIndex];
            string indent = closeBraceLine.Substring(0, closeBraceLine.Length - closeBraceLine.TrimStart().Length);
            string extraIndent = indent + "    ";

            string[] codeLines = code.Replace("\r\n", "\n").Split('\n');
            var newLines = new List<string>();
            newLines.Add(indent);
            foreach (string codeLine in codeLines)
            {
                if (string.IsNullOrEmpty(codeLine))
                    newLines.Add("");
                else
                    newLines.Add(extraIndent + codeLine);
            }

            var updatedLines = new List<string>(lines);
            updatedLines.InsertRange(classEndLineIndex, newLines);

            File.WriteAllLines(fullPath, updatedLines);
            if (AutoRefresh) AssetDatabase.Refresh();
            return $"Inserted code before end of class in {relativePath}";
        }

        private static string InsertAfterLine(string relativePath, string searchLineContains, string code, string baseDir)
        {
            if (!IsPathAllowed(relativePath))
                return $"Access denied: '{relativePath}' is not allowed by safety rules.";
            if (string.IsNullOrEmpty(searchLineContains))
                return "Search line substring is empty";
            string fullPath = Path.Combine(baseDir, relativePath);
            if (!File.Exists(fullPath))
                return $"File not found: {fullPath}";

            string[] lines = File.ReadAllLines(fullPath);
            int targetIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(searchLineContains))
                {
                    targetIndex = i;
                    break;
                }
            }
            if (targetIndex == -1)
                return $"Line containing '{searchLineContains}' not found in {relativePath}";

            string targetLine = lines[targetIndex];
            string indent = targetLine.Substring(0, targetLine.Length - targetLine.TrimStart().Length);

            string[] codeLines = code.Replace("\r\n", "\n").Split('\n');
            var newLines = new List<string>();
            foreach (string codeLine in codeLines)
            {
                if (string.IsNullOrEmpty(codeLine))
                    newLines.Add("");
                else
                    newLines.Add(indent + codeLine);
            }

            var updatedLines = new List<string>(lines);
            updatedLines.InsertRange(targetIndex + 1, newLines);

            File.WriteAllLines(fullPath, updatedLines);
            if (AutoRefresh) AssetDatabase.Refresh();
            return $"Inserted code after line containing '{searchLineContains}' in {relativePath}";
        }
    }

    // Окно настроек безопасности и путей по умолчанию
    public class SafetySettingsWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private string newAllowedItem = "";
        private string newBlockedItem = "";

        void OnDestroy()
        {
            PatchApplier.SaveSettings();
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.LabelField("PatchApplier Safety Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Allowed Path Prefixes (if any, only these are allowed):");
            EditorGUI.indentLevel++;
            for (int i = 0; i < PatchApplier.AllowedPathPrefixes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                PatchApplier.AllowedPathPrefixes[i] = EditorGUILayout.TextField(PatchApplier.AllowedPathPrefixes[i]);
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    PatchApplier.AllowedPathPrefixes.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            newAllowedItem = EditorGUILayout.TextField(newAllowedItem);
            if (GUILayout.Button("Add", GUILayout.Width(60)) && !string.IsNullOrWhiteSpace(newAllowedItem))
            {
                PatchApplier.AllowedPathPrefixes.Add(newAllowedItem);
                newAllowedItem = "";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Blocked Path Prefixes (ignored if Allowed list is not empty):");
            EditorGUI.indentLevel++;
            for (int i = 0; i < PatchApplier.BlockedPathPrefixes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                PatchApplier.BlockedPathPrefixes[i] = EditorGUILayout.TextField(PatchApplier.BlockedPathPrefixes[i]);
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    PatchApplier.BlockedPathPrefixes.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            newBlockedItem = EditorGUILayout.TextField(newBlockedItem);
            if (GUILayout.Button("Add", GUILayout.Width(60)) && !string.IsNullOrWhiteSpace(newBlockedItem))
            {
                PatchApplier.BlockedPathPrefixes.Add(newBlockedItem);
                newBlockedItem = "";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Default Patch Paths", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Leave empty to auto-detect (next to script).");

            EditorGUILayout.LabelField("JSON Patch:");
            EditorGUILayout.BeginHorizontal();
            PatchApplier.CustomDefaultJsonPath = EditorGUILayout.TextField(PatchApplier.CustomDefaultJsonPath ?? "");
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selected = EditorUtility.OpenFilePanel("Select JSON patch", Application.dataPath, "json");
                if (!string.IsNullOrEmpty(selected))
                    PatchApplier.CustomDefaultJsonPath = selected;
            }
            if (GUILayout.Button("Reset", GUILayout.Width(60)))
            {
                PatchApplier.CustomDefaultJsonPath = null;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("YAML Patch:");
            EditorGUILayout.BeginHorizontal();
            PatchApplier.CustomDefaultYamlPath = EditorGUILayout.TextField(PatchApplier.CustomDefaultYamlPath ?? "");
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selected = EditorUtility.OpenFilePanel("Select YAML patch", Application.dataPath, "yaml,yml");
                if (!string.IsNullOrEmpty(selected))
                    PatchApplier.CustomDefaultYamlPath = selected;
            }
            if (GUILayout.Button("Reset", GUILayout.Width(60)))
            {
                PatchApplier.CustomDefaultYamlPath = null;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Кнопка Reset to Defaults
            if (GUILayout.Button("Reset to Defaults"))
            {
                if (EditorUtility.DisplayDialog("Reset to Defaults",
                    "Are you sure you want to reset all PatchApplier settings to default?", "Yes", "No"))
                {
                    PatchApplier.ResetToDefaults();
                    // Окно обновится автоматически в следующем кадре
                    Repaint();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Settings are saved automatically when this window closes.", MessageType.Info);

            EditorGUILayout.EndScrollView();
        }
    }
}
