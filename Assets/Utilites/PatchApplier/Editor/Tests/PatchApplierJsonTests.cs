using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

namespace PatchProcessing.Tests
{
    [TestFixture]
    public class PatchApplierTests
    {
        private const string TestDir = "Assets/TempTestData";
        private string fullTestDir;
        private List<string> savedAllowed;
        private List<string> savedBlocked;

        [SetUp]
        public void SetUp()
        {
            savedAllowed = new List<string>(PatchApplier.AllowedPathPrefixes);
            savedBlocked = new List<string>(PatchApplier.BlockedPathPrefixes);
            PatchApplier.AllowedPathPrefixes.Clear();
            PatchApplier.BlockedPathPrefixes = new List<string> { "Plugins/", "Resources/", "StreamingAssets/", "Standard Assets/" };

            // Отключаем автообновление, чтобы не сбрасывать консоль и не тормозить тесты
            PatchApplier.AutoRefresh = false;

            fullTestDir = Path.Combine(Application.dataPath, "TempTestData");
            if (!Directory.Exists(fullTestDir))
                Directory.CreateDirectory(fullTestDir);
            foreach (var file in Directory.GetFiles(fullTestDir))
                File.Delete(file);
            foreach (var file in Directory.GetFiles(fullTestDir, "*.meta"))
                File.Delete(file);
            // AssetDatabase.Refresh() не вызываем, чтобы избежать перекомпиляции и очистки консоли
        }

        [TearDown]
        public void TearDown()
        {
            PatchApplier.AllowedPathPrefixes = savedAllowed;
            PatchApplier.BlockedPathPrefixes = savedBlocked;
            PatchApplier.AutoRefresh = true; // восстанавливаем значение по умолчанию

            if (Directory.Exists(fullTestDir))
            {
                Directory.Delete(fullTestDir, true);
                string metaPath = fullTestDir + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
                // Не вызываем AssetDatabase.Refresh() – файлы удалены напрямую
            }
        }

        private string ApplySinglePatch(Patch patch)
        {
            var patches = new List<Patch> { patch };
            string logMessage = null;
            Application.LogCallback callback = (condition, stackTrace, type) =>
            {
                if (type == LogType.Log && condition.StartsWith("["))
                    logMessage = condition;
            };
            Application.logMessageReceived += callback;
            PatchApplier.ApplyPatches(patches);
            Application.logMessageReceived -= callback;
            return logMessage;
        }

        // ------------------------------------------------------------
        // Старые тесты (20 штук)
        // ------------------------------------------------------------

        [Test]
        public void CreateFile_Success()
        {
            var patch = new Patch { action = "create_file", arguments = new Arguments { file_path = "TempTestData/hello.txt", file_content = "Hello, world!" } };
            string result = ApplySinglePatch(patch);
            Assert.IsTrue(File.Exists(Path.Combine(Application.dataPath, "TempTestData/hello.txt")));
            Assert.AreEqual("Hello, world!", File.ReadAllText(Path.Combine(Application.dataPath, "TempTestData/hello.txt")));
            StringAssert.Contains("File created", result);
        }

        [Test]
        public void CreateFile_AlreadyExists_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "existing.txt"), "test");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "create_file", arguments = new Arguments { file_path = "TempTestData/existing.txt", file_content = "new" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("already exists", result);
            Assert.AreEqual("test", File.ReadAllText(Path.Combine(fullTestDir, "existing.txt")));
        }

        [Test]
        public void OverwriteFile_Success()
        {
            var patch = new Patch { action = "overwrite_file", arguments = new Arguments { file_path = "TempTestData/overwrite.txt", file_content = "overwritten" } };
            ApplySinglePatch(patch);
            Assert.AreEqual("overwritten", File.ReadAllText(Path.Combine(fullTestDir, "overwrite.txt")));
        }

        [Test]
        public void DeleteFile_Success()
        {
            string filePath = Path.Combine(fullTestDir, "to_delete.txt");
            File.WriteAllText(filePath, "delete me");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "delete_file", arguments = new Arguments { file_path = "TempTestData/to_delete.txt" } };
            string result = ApplySinglePatch(patch);
            Assert.IsFalse(File.Exists(filePath));
            Assert.IsFalse(File.Exists(filePath + ".meta"));
            StringAssert.Contains("File deleted", result);
        }

        [Test]
        public void DeleteFile_NotFound_Error()
        {
            var patch = new Patch { action = "delete_file", arguments = new Arguments { file_path = "TempTestData/nonexistent.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("File not found", result);
        }

        [Test]
        public void DeleteFile_RemovesMetaFile()
        {
            string filePath = Path.Combine(fullTestDir, "with_meta.txt");
            File.WriteAllText(filePath, "data");
            string metaGuid = System.Guid.NewGuid().ToString("N");
            string metaContent = "fileFormatVersion: 2\nguid: " + metaGuid + "\n";
            File.WriteAllText(filePath + ".meta", metaContent);
            AssetDatabase.Refresh();
            Assert.IsTrue(File.Exists(filePath + ".meta"));
            var patch = new Patch { action = "delete_file", arguments = new Arguments { file_path = "TempTestData/with_meta.txt" } };
            ApplySinglePatch(patch);
            Assert.IsFalse(File.Exists(filePath));
            Assert.IsFalse(File.Exists(filePath + ".meta"));
        }

        [Test]
        public void ReplaceInFile_FirstOccurrence_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "replace.txt"), "Hello Hello Hello");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "replace_in_file", arguments = new Arguments { file_path = "TempTestData/replace.txt", search = "Hello", replace = "Hi", occurrence = 1 } };
            ApplySinglePatch(patch);
            string content = File.ReadAllText(Path.Combine(fullTestDir, "replace.txt"));
            Assert.AreEqual("Hi Hello Hello", content);
        }

        [Test]
        public void ReplaceInFile_AllOccurrences_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "replace_all.txt"), "Hello Hello Hello");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "replace_in_file", arguments = new Arguments { file_path = "TempTestData/replace_all.txt", search = "Hello", replace = "Hey", occurrence = "all" } };
            ApplySinglePatch(patch);
            string content = File.ReadAllText(Path.Combine(fullTestDir, "replace_all.txt"));
            Assert.AreEqual("Hey Hey Hey", content);
        }

        [Test]
        public void ReplaceInFile_NotFound_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "not_found.txt"), "Hi Hey Hey");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "replace_in_file", arguments = new Arguments { file_path = "TempTestData/not_found.txt", search = "Hello", replace = "X", occurrence = 2 } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("not found", result);
        }

        [Test]
        public void ReplaceInFile_InvalidOccurrence_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "invalid_occ.txt"), "Hey");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "replace_in_file", arguments = new Arguments { file_path = "TempTestData/invalid_occ.txt", search = "Hey", replace = "Y", occurrence = "invalid" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Invalid occurrence value", result);
        }

        [Test]
        public void MoveFile_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "source.txt"), "move me");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "move_file", arguments = new Arguments { source_path = "TempTestData/source.txt", destination_path = "TempTestData/dest.txt" } };
            string result = ApplySinglePatch(patch);
            Assert.IsFalse(File.Exists(Path.Combine(fullTestDir, "source.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(fullTestDir, "dest.txt")));
            Assert.AreEqual("move me", File.ReadAllText(Path.Combine(fullTestDir, "dest.txt")));
            StringAssert.Contains("Moved", result);
        }

        [Test]
        public void MoveFile_SourceNotFound_Error()
        {
            var patch = new Patch { action = "move_file", arguments = new Arguments { source_path = "TempTestData/no_source.txt", destination_path = "TempTestData/dest.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Source file not found", result);
        }

        [Test]
        public void MoveFile_DestinationExists_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "source.txt"), "src");
            File.WriteAllText(Path.Combine(fullTestDir, "dest.txt"), "dst");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "move_file", arguments = new Arguments { source_path = "TempTestData/source.txt", destination_path = "TempTestData/dest.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Destination already exists", result);
            Assert.IsTrue(File.Exists(Path.Combine(fullTestDir, "source.txt")));
        }

        [Test]
        public void ListFiles_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "file1.txt"), "");
            File.WriteAllText(Path.Combine(fullTestDir, "file2.txt"), "");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "list_files", arguments = new Arguments { directory = "TempTestData" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("file1.txt", result);
            StringAssert.Contains("file2.txt", result);
        }

        [Test]
        public void ListFiles_NotFound_Error()
        {
            var patch = new Patch { action = "list_files", arguments = new Arguments { directory = "NoDir" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Directory not found", result);
        }

        [Test]
        public void ReadFile_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "read.txt"), "content");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "read_file", arguments = new Arguments { file_path = "TempTestData/read.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("content", result);
        }

        [Test]
        public void ReadFile_NotFound_Error()
        {
            var patch = new Patch { action = "read_file", arguments = new Arguments { file_path = "TempTestData/nonexistent.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("File not found", result);
        }

        [Test]
        public void InvalidPatch_NoAction_Skipped()
        {
            var patch = new Patch { arguments = new Arguments { file_path = "TempTestData/x.txt" } };
            string result = ApplySinglePatch(patch);
            Assert.IsNull(result);
        }

        [Test]
        public void InvalidPatch_NoArguments_Error()
        {
            var patch = new Patch { action = "create_file" };
            LogAssert.Expect(LogType.Error, "[1] create_file: ERROR - Missing arguments");
            ApplySinglePatch(patch);
        }

        [Test]
        public void UnknownAction_Error()
        {
            var patch = new Patch { action = "unknown_action", arguments = new Arguments { file_path = "x" } };
            LogAssert.Expect(LogType.Error, "[1] unknown_action: ERROR - Unknown action: unknown_action");
            ApplySinglePatch(patch);
        }

        // ------------------------------------------------------------
        // Тесты безопасности
        // ------------------------------------------------------------

        [Test]
        public void DefaultBlacklist_ContainsExpectedFolders()
        {
            Assert.Contains("Plugins/", PatchApplier.BlockedPathPrefixes);
            Assert.Contains("Resources/", PatchApplier.BlockedPathPrefixes);
            Assert.Contains("StreamingAssets/", PatchApplier.BlockedPathPrefixes);
            Assert.Contains("Standard Assets/", PatchApplier.BlockedPathPrefixes);
        }

        [Test]
        public void CreateFile_BlockedPath_AccessDenied()
        {
            var patch = new Patch { action = "create_file", arguments = new Arguments { file_path = "Plugins/forbidden.txt", file_content = "test" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Access denied", result);
            Assert.IsFalse(File.Exists(Path.Combine(Application.dataPath, "Plugins/forbidden.txt")));
        }

        [Test]
        public void MoveFile_BlockedDestination_AccessDenied()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "source.txt"), "test");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "move_file", arguments = new Arguments { source_path = "TempTestData/source.txt", destination_path = "Resources/dest.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Access denied", result);
            Assert.IsTrue(File.Exists(Path.Combine(fullTestDir, "source.txt")));
        }

        [Test]
        public void AllowedList_OverridesBlocked()
        {
            PatchApplier.AllowedPathPrefixes.Clear();
            PatchApplier.AllowedPathPrefixes.Add("TempTestData/");
            PatchApplier.BlockedPathPrefixes.Clear();

            var patchOk = new Patch { action = "create_file", arguments = new Arguments { file_path = "TempTestData/allowed.txt", file_content = "ok" } };
            string resultOk = ApplySinglePatch(patchOk);
            StringAssert.Contains("File created", resultOk);

            var patchDenied = new Patch { action = "create_file", arguments = new Arguments { file_path = "Plugins/denied.txt", file_content = "no" } };
            string resultDenied = ApplySinglePatch(patchDenied);
            StringAssert.Contains("Access denied", resultDenied);
        }

        [Test]
        public void DeleteFile_EmptyAllowedList_Denied()
        {
            var patch = new Patch { action = "delete_file", arguments = new Arguments { file_path = "Plugins/something.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Access denied", result);
        }

        [Test]
        public void ReadFile_AllowedEvenWhenBlacklistEmpty()
        {
            PatchApplier.BlockedPathPrefixes.Clear();
            PatchApplier.AllowedPathPrefixes.Clear();
            File.WriteAllText(Path.Combine(fullTestDir, "read_any.txt"), "data");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "read_file", arguments = new Arguments { file_path = "TempTestData/read_any.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("data", result);
        }

        // ------------------------------------------------------------
        // Новые тесты: file_exists, insert_before_class_end, insert_after_line
        // ------------------------------------------------------------

        [Test]
        public void FileExists_ReturnsTrue()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "exists.txt"), "");
            AssetDatabase.Refresh();
            var patch = new Patch { action = "file_exists", arguments = new Arguments { file_path = "TempTestData/exists.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("true", result);
        }

        [Test]
        public void FileExists_ReturnsFalse()
        {
            var patch = new Patch { action = "file_exists", arguments = new Arguments { file_path = "TempTestData/no.txt" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("false", result);
        }

        [Test]
        public void InsertBeforeClassEnd_SimpleClass_InsertsMethod()
        {
            string classCode = @"using UnityEngine;
public class TestClass : MonoBehaviour
{
    void Start() { }
}";
            File.WriteAllText(Path.Combine(fullTestDir, "TestClass.cs.txt"), classCode);

            string methodCode = @"void OnEnable()
{
    Debug.Log(""Enabled"");
}";
            var patch = new Patch { action = "insert_before_class_end", arguments = new Arguments { file_path = "TempTestData/TestClass.cs.txt", class_name = "TestClass", file_content = methodCode } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Inserted code before end of class", result);

            string updated = File.ReadAllText(Path.Combine(fullTestDir, "TestClass.cs.txt"));
            StringAssert.Contains("void OnEnable()", updated);
            StringAssert.Contains("Debug.Log(\"Enabled\")", updated);
            int lastBraceIndex = updated.LastIndexOf('}');
            int methodIndex = updated.IndexOf("void OnEnable()");
            Assert.IsTrue(methodIndex < lastBraceIndex);
        }

        [Test]
        public void InsertAfterLine_InsertsCode()
        {
            string original = @"line1
line2
target line
line3";
            File.WriteAllText(Path.Combine(fullTestDir, "insert_test.txt"), original);
            AssetDatabase.Refresh();

            var patch = new Patch { action = "insert_after_line", arguments = new Arguments { file_path = "TempTestData/insert_test.txt", search_line_contains = "target", file_content = "INSERTED" } };
            string result = ApplySinglePatch(patch);
            StringAssert.Contains("Inserted code after line containing 'target'", result);

            string updated = File.ReadAllText(Path.Combine(fullTestDir, "insert_test.txt"));
            string[] updatedLines = updated.Replace("\r\n", "\n").Split('\n');
            bool found = false;
            for (int i = 0; i < updatedLines.Length - 2; i++)
            {
                if (updatedLines[i].Trim() == "target line" &&
                    updatedLines[i+1].Trim() == "INSERTED" &&
                    updatedLines[i+2].Trim() == "line3")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Expected 'INSERTED' after 'target line' before 'line3'");
        }
    }
}
