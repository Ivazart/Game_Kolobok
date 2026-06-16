using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEditor;
using UnityEngine;

public class CodeCollectorWindow : EditorWindow
{
    // ---- Состояние UI ----
    private Vector2 scroll;
    private List<string> sourceFolders = new List<string> {"Assets"};
    private List<string> fileExtensionWhitelist = new List<string> {".cs", ".shader", ".compute", ".hlsl"};
    private List<string> fileExtensionBlacklist = new List<string>();
    private bool useWhitelist = true;
    private List<string> excludedDirectories = new List<string> {".git", "Library", "Temp", "Obj", "Build"};
    private List<string> excludedFiles = new List<string> {"combined_code.txt"};
    private string outputPath = "Assets/combined_code.txt";
    private bool includeUsings = true;
    private bool includeXmlDocs = true;

    private enum CollectionMode
    {
        Full,
        PublicInterfaceOnly
    }

    private CollectionMode mode = CollectionMode.Full;

    [MenuItem("Tools/Code Collector")]
    public static void ShowWindow() => GetWindow<CodeCollectorWindow>("Code Collector");

    private void OnEnable()
    {
        minSize = new Vector2(400, 650);
        LoadSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.LabelField("Режим сбора", EditorStyles.boldLabel);
        mode = (CollectionMode) EditorGUILayout.EnumPopup("Режим:", mode);
        EditorGUILayout.Space();

        if (mode == CollectionMode.PublicInterfaceOnly)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Настройки публичного интерфейса", EditorStyles.boldLabel);
            includeUsings = EditorGUILayout.Toggle("Include using directives", includeUsings);
            includeXmlDocs = EditorGUILayout.Toggle("Include XML doc comments (///)", includeXmlDocs);
        }

        // --- Папки-источники ---
        EditorGUILayout.LabelField("Папки для сбора", EditorStyles.boldLabel);
        for (int i = 0; i < sourceFolders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            sourceFolders[i] = EditorGUILayout.TextField(sourceFolders[i]);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string selected = EditorUtility.OpenFolderPanel("Выбери папку", "Assets", "");
                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected.StartsWith(Application.dataPath))
                        sourceFolders[i] = "Assets" + selected.Substring(Application.dataPath.Length);
                    else
                        Debug.LogWarning("Папка должна находиться внутри проекта!");
                }
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
                sourceFolders.RemoveAt(i--);
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Добавить папку"))
            sourceFolders.Add("Assets");
        EditorGUILayout.Space();

        // --- Фильтры расширений ---
        EditorGUILayout.LabelField("Фильтр расширений", EditorStyles.boldLabel);
        useWhitelist = EditorGUILayout.Toggle("Использовать белый список (иначе чёрный)", useWhitelist);
        if (useWhitelist)
        {
            EditorGUILayout.LabelField("Только эти расширения:");
            DrawStringList(fileExtensionWhitelist, ".cs");
        }
        else
        {
            EditorGUILayout.LabelField("Исключить эти расширения:");
            DrawStringList(fileExtensionBlacklist, ".meta");
        }

        EditorGUILayout.Space();

        // --- Исключённые папки / файлы ---
        EditorGUILayout.LabelField("Исключения", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Исключённые папки:");
        DrawStringList(excludedDirectories, ".git");
        EditorGUILayout.LabelField("Исключённые файлы (полное имя):");
        DrawStringList(excludedFiles, "combined_code.txt");
        EditorGUILayout.Space();

        // --- Выходной файл ---
        outputPath = EditorGUILayout.TextField("Выходной файл:", outputPath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string save = EditorUtility.SaveFilePanel("Сохранить результат", "Assets", "combined_code.txt", "txt");
            if (!string.IsNullOrEmpty(save) && save.StartsWith(Application.dataPath))
                outputPath = "Assets" + save.Substring(Application.dataPath.Length);
            else
                Debug.LogWarning("Сохранять нужно внутри проекта (Assets/...)");
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Reset to Defaults", GUILayout.Height(25)))
            ResetToDefaults();
        if (GUILayout.Button("Собрать код", GUILayout.Height(30)))
            CollectCode();

        EditorGUILayout.EndScrollView();
    }

    private void DrawStringList(List<string> list, string defaultNewValue)
    {
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            list[i] = EditorGUILayout.TextField(list[i]);
            if (GUILayout.Button("-", GUILayout.Width(20)))
                list.RemoveAt(i--);
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+"))
            list.Add(defaultNewValue);
    }

    private void CollectCode()
    {
        try
        {
            var sb = new StringBuilder();
            var extensionsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var excludedDirsSet = new HashSet<string>(excludedDirectories, StringComparer.OrdinalIgnoreCase);
            var excludedFilesSet = new HashSet<string>(excludedFiles, StringComparer.OrdinalIgnoreCase);

            if (useWhitelist)
                extensionsSet.UnionWith(fileExtensionWhitelist);
            else
            {
                if (fileExtensionBlacklist.Count == 0)
                {
                    var defaultBlacklist = new[]
                        {".meta", ".asset", ".prefab", ".unity", ".mat", ".png", ".jpg", ".fbx", ".dll"};
                    extensionsSet.UnionWith(defaultBlacklist);
                }
                else
                    extensionsSet.UnionWith(fileExtensionBlacklist);
            }

            foreach (string relativeFolder in sourceFolders)
            {
                string fullFolder = Path.Combine(Application.dataPath,
                    relativeFolder.StartsWith("Assets/") ? relativeFolder.Substring(7) : relativeFolder);
                if (!Directory.Exists(fullFolder))
                {
                    Debug.LogWarning($"Папка не найдена: {fullFolder}");
                    continue;
                }

                var allFiles = Directory.GetFiles(fullFolder, "*.*", SearchOption.AllDirectories);
                foreach (string filePath in allFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    string extension = Path.GetExtension(filePath);

                    string relativeDir = Path.GetDirectoryName(filePath)
                        .Replace(Application.dataPath + Path.DirectorySeparatorChar,
                            "Assets" + Path.DirectorySeparatorChar).Replace("\\", "/");
                    if (excludedDirsSet.Any(dir =>
                            relativeDir.Contains("/" + dir + "/") || relativeDir.EndsWith("/" + dir) ||
                            relativeDir == dir))
                        continue;

                    if (excludedFilesSet.Contains(fileName))
                        continue;

                    if (useWhitelist && !extensionsSet.Contains(extension))
                        continue;
                    if (!useWhitelist && extensionsSet.Contains(extension))
                        continue;

                    string content;
                    try
                    {
                        content = File.ReadAllText(filePath);
                    }
                    catch (Exception e)
                    {
                        content = $"[ERROR reading file: {e.Message}]";
                    }

                    if (mode == CollectionMode.PublicInterfaceOnly &&
                        extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        content = ExtractPublicInterface(content);
                    }

                    string relativePath = "Assets" + filePath.Substring(Application.dataPath.Length).Replace("\\", "/");
                    sb.AppendLine(relativePath);
                    sb.AppendLine("***");
                    sb.AppendLine(content);
                    sb.AppendLine();
                }
            }

            string fullOutput = Path.Combine(Application.dataPath,
                outputPath.StartsWith("Assets/") ? outputPath.Substring(7) : outputPath);
            File.WriteAllText(fullOutput, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            Debug.Log($"Готово! Результат сохранён в: {outputPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сборе кода: {ex}");
        }
    }

    private string ExtractPublicInterface(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        var rewriter = new PublicInterfaceRewriter(includeUsings, includeXmlDocs);
        var newRoot = rewriter.Visit(root);
        string result = newRoot.ToFullString();

        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+;", ";");

        if (!includeXmlDocs)
        {
            result = System.Text.RegularExpressions.Regex.Replace(result, @"^\s*///.*$", "", System.Text.RegularExpressions.RegexOptions.Multiline);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\n\s*\n", "\n");
        }

        return result;
    }

    private class PublicInterfaceRewriter : CSharpSyntaxRewriter
    {
        private readonly bool _includeUsings;

        public PublicInterfaceRewriter(bool includeUsings = true, bool includeXmlDocs = true)
        {
            _includeUsings = includeUsings;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return base.VisitStructDeclaration(node);
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return base.VisitInterfaceDeclaration(node);
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return base.VisitEnumDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node.WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(node.GetTrailingTrivia());
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            var accessors = node.AccessorList?.Accessors;
            if (accessors != null && accessors.Value.Any())
            {
                var newAccessors = accessors.Value.Select(acc =>
                    acc.WithBody(null)
                        .WithExpressionBody(null)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                node = node.WithAccessorList(SyntaxFactory.AccessorList(
                    new SyntaxList<AccessorDeclarationSyntax>(newAccessors)));
            }
            node = node.WithExpressionBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            return node;
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node;
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node.WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node;
        }

        public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node;
        }

        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node.WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        public override SyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            return node.WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return null;
            var accessors = node.AccessorList?.Accessors;
            if (accessors != null && accessors.Value.Any())
            {
                var newAccessors = accessors.Value.Select(acc =>
                    acc.WithBody(null).WithExpressionBody(null)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                node = node.WithAccessorList(SyntaxFactory.AccessorList(
                    new SyntaxList<AccessorDeclarationSyntax>(newAccessors)));
            }
            node = node.WithExpressionBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            return node;
        }

        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (!_includeUsings)
                return null;
            return node;
        }
    }

    [Serializable]
    private class Settings
    {
        public List<string> sourceFolders;
        public List<string> fileExtensionWhitelist;
        public List<string> fileExtensionBlacklist;
        public bool useWhitelist;
        public List<string> excludedDirectories;
        public List<string> excludedFiles;
        public string outputPath;
        public bool includeUsings;
        public bool includeXmlDocs;
        public int mode;
    }

    private void SaveSettings()
    {
        var s = new Settings
        {
            sourceFolders = this.sourceFolders,
            fileExtensionWhitelist = this.fileExtensionWhitelist,
            fileExtensionBlacklist = this.fileExtensionBlacklist,
            useWhitelist = this.useWhitelist,
            excludedDirectories = this.excludedDirectories,
            excludedFiles = this.excludedFiles,
            outputPath = this.outputPath,
            includeUsings = this.includeUsings,
            includeXmlDocs = this.includeXmlDocs,
            mode = this.mode == CollectionMode.PublicInterfaceOnly ? 1 : 0
        };
        EditorPrefs.SetString("CodeCollectorSettings", JsonUtility.ToJson(s));
    }

    private void LoadSettings()
    {
        string json = EditorPrefs.GetString("CodeCollectorSettings", "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var s = JsonUtility.FromJson<Settings>(json);
                if (s.sourceFolders != null) this.sourceFolders = s.sourceFolders;
                if (s.fileExtensionWhitelist != null) this.fileExtensionWhitelist = s.fileExtensionWhitelist;
                if (s.fileExtensionBlacklist != null) this.fileExtensionBlacklist = s.fileExtensionBlacklist;
                this.useWhitelist = s.useWhitelist;
                if (s.excludedDirectories != null) this.excludedDirectories = s.excludedDirectories;
                if (s.excludedFiles != null) this.excludedFiles = s.excludedFiles;
                if (!string.IsNullOrEmpty(s.outputPath)) this.outputPath = s.outputPath;
                this.includeUsings = s.includeUsings;
                this.includeXmlDocs = s.includeXmlDocs;
                this.mode = s.mode == 1 ? CollectionMode.PublicInterfaceOnly : CollectionMode.Full;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load CodeCollector settings: {e.Message}");
            }
        }
    }

    private void ResetToDefaults()
    {
        sourceFolders = new List<string> {"Assets"};
        fileExtensionWhitelist = new List<string> {".cs", ".shader", ".compute", ".hlsl"};
        fileExtensionBlacklist = new List<string>();
        useWhitelist = true;
        excludedDirectories = new List<string> {".git", "Library", "Temp", "Obj", "Build"};
        excludedFiles = new List<string> {"combined_code.txt"};
        outputPath = "Assets/combined_code.txt";
        includeUsings = true;
        includeXmlDocs = true;
        mode = CollectionMode.Full;
        SaveSettings();
        Debug.Log("Code Collector settings reset to defaults.");
        Repaint();
    }
}