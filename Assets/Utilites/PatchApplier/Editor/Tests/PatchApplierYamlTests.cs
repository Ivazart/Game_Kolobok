using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

namespace PatchProcessing.Tests
{
    [TestFixture]
    public class PatchApplierYamlTests
    {
        private const string TestDir = "Assets/TempTestDataYaml";
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

            PatchApplier.AutoRefresh = false;

            fullTestDir = Path.Combine(Application.dataPath, "TempTestDataYaml");
            if (!Directory.Exists(fullTestDir))
                Directory.CreateDirectory(fullTestDir);
            foreach (var file in Directory.GetFiles(fullTestDir))
                File.Delete(file);
            foreach (var file in Directory.GetFiles(fullTestDir, "*.meta"))
                File.Delete(file);
            // Не вызываем AssetDatabase.Refresh()
        }

        [TearDown]
        public void TearDown()
        {
            PatchApplier.AllowedPathPrefixes = savedAllowed;
            PatchApplier.BlockedPathPrefixes = savedBlocked;
            PatchApplier.AutoRefresh = true;

            if (Directory.Exists(fullTestDir))
            {
                Directory.Delete(fullTestDir, true);
                string metaPath = fullTestDir + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
                // Не вызываем AssetDatabase.Refresh()
            }
        }

        private string ApplySingleYamlPatch(string yaml)
        {
            var patches = SimpleYamlParser.Parse(yaml);
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
        // Старые YAML-тесты (включая безопасность)
        // ------------------------------------------------------------

        [Test]
        public void CreateFile_Success()
        {
            string yaml = @"
- action: create_file
  arguments:
    file_path: TempTestDataYaml/hello.txt
    file_content: |
      Hello, world!
";
            string result = ApplySingleYamlPatch(yaml);
            Assert.IsTrue(File.Exists(Path.Combine(Application.dataPath, "TempTestDataYaml/hello.txt")));
            Assert.AreEqual("Hello, world!", File.ReadAllText(Path.Combine(Application.dataPath, "TempTestDataYaml/hello.txt")));
            StringAssert.Contains("File created", result);
        }

        [Test]
        public void CreateFile_AlreadyExists_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "existing.txt"), "test");
            AssetDatabase.Refresh();
            string yaml = @"
- action: create_file
  arguments:
    file_path: TempTestDataYaml/existing.txt
    file_content: new
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("already exists", result);
            Assert.AreEqual("test", File.ReadAllText(Path.Combine(fullTestDir, "existing.txt")));
        }

        [Test]
        public void OverwriteFile_Success()
        {
            string yaml = @"
- action: overwrite_file
  arguments:
    file_path: TempTestDataYaml/overwrite.txt
    file_content: overwritten
";
            ApplySingleYamlPatch(yaml);
            Assert.AreEqual("overwritten", File.ReadAllText(Path.Combine(fullTestDir, "overwrite.txt")));
        }

        [Test]
        public void DeleteFile_Success()
        {
            string filePath = Path.Combine(fullTestDir, "to_delete.txt");
            File.WriteAllText(filePath, "delete me");
            AssetDatabase.Refresh();
            string yaml = @"
- action: delete_file
  arguments:
    file_path: TempTestDataYaml/to_delete.txt
";
            string result = ApplySingleYamlPatch(yaml);
            Assert.IsFalse(File.Exists(filePath));
            Assert.IsFalse(File.Exists(filePath + ".meta"));
            StringAssert.Contains("File deleted", result);
        }

        [Test]
        public void DeleteFile_NotFound_Error()
        {
            string yaml = @"
- action: delete_file
  arguments:
    file_path: TempTestDataYaml/nonexistent.txt
";
            string result = ApplySingleYamlPatch(yaml);
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
            string yaml = @"
- action: delete_file
  arguments:
    file_path: TempTestDataYaml/with_meta.txt
";
            ApplySingleYamlPatch(yaml);
            Assert.IsFalse(File.Exists(filePath));
            Assert.IsFalse(File.Exists(filePath + ".meta"));
        }

        [Test]
        public void ReplaceInFile_FirstOccurrence_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "replace.txt"), "Hello Hello Hello");
            AssetDatabase.Refresh();
            string yaml = @"
- action: replace_in_file
  arguments:
    file_path: TempTestDataYaml/replace.txt
    search: Hello
    replace: Hi
    occurrence: 1
";
            ApplySingleYamlPatch(yaml);
            string content = File.ReadAllText(Path.Combine(fullTestDir, "replace.txt"));
            Assert.AreEqual("Hi Hello Hello", content);
        }

        [Test]
        public void ReplaceInFile_AllOccurrences_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "replace_all.txt"), "Hello Hello Hello");
            AssetDatabase.Refresh();
            string yaml = @"
- action: replace_in_file
  arguments:
    file_path: TempTestDataYaml/replace_all.txt
    search: Hello
    replace: Hey
    occurrence: all
";
            ApplySingleYamlPatch(yaml);
            string content = File.ReadAllText(Path.Combine(fullTestDir, "replace_all.txt"));
            Assert.AreEqual("Hey Hey Hey", content);
        }

        [Test]
        public void ReplaceInFile_SecondOccurrence_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "replace_second.txt"), "AAA BBB AAA");
            AssetDatabase.Refresh();
            string yaml = @"
- action: replace_in_file
  arguments:
    file_path: TempTestDataYaml/replace_second.txt
    search: AAA
    replace: ZZZ
    occurrence: 2
";
            ApplySingleYamlPatch(yaml);
            string content = File.ReadAllText(Path.Combine(fullTestDir, "replace_second.txt"));
            Assert.AreEqual("AAA BBB ZZZ", content);
        }

        [Test]
        public void ReplaceInFile_NotFound_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "not_found.txt"), "Hi Hey Hey");
            AssetDatabase.Refresh();
            string yaml = @"
- action: replace_in_file
  arguments:
    file_path: TempTestDataYaml/not_found.txt
    search: Hello
    replace: X
    occurrence: 2
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("not found", result);
        }

        [Test]
        public void ReplaceInFile_InvalidOccurrence_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "invalid_occ.txt"), "Hey");
            AssetDatabase.Refresh();
            string yaml = @"
- action: replace_in_file
  arguments:
    file_path: TempTestDataYaml/invalid_occ.txt
    search: Hey
    replace: Y
    occurrence: invalid
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Invalid occurrence value", result);
        }

        [Test]
        public void MoveFile_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "source.txt"), "move me");
            AssetDatabase.Refresh();
            string yaml = @"
- action: move_file
  arguments:
    source_path: TempTestDataYaml/source.txt
    destination_path: TempTestDataYaml/dest.txt
";
            string result = ApplySingleYamlPatch(yaml);
            Assert.IsFalse(File.Exists(Path.Combine(fullTestDir, "source.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(fullTestDir, "dest.txt")));
            Assert.AreEqual("move me", File.ReadAllText(Path.Combine(fullTestDir, "dest.txt")));
            StringAssert.Contains("Moved", result);
        }

        [Test]
        public void MoveFile_SourceNotFound_Error()
        {
            string yaml = @"
- action: move_file
  arguments:
    source_path: TempTestDataYaml/no_source.txt
    destination_path: TempTestDataYaml/dest.txt
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Source file not found", result);
        }

        [Test]
        public void MoveFile_DestinationExists_Error()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "source.txt"), "src");
            File.WriteAllText(Path.Combine(fullTestDir, "dest.txt"), "dst");
            AssetDatabase.Refresh();
            string yaml = @"
- action: move_file
  arguments:
    source_path: TempTestDataYaml/source.txt
    destination_path: TempTestDataYaml/dest.txt
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Destination already exists", result);
            Assert.IsTrue(File.Exists(Path.Combine(fullTestDir, "source.txt")));
        }

        [Test]
        public void ListFiles_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "file1.txt"), "");
            File.WriteAllText(Path.Combine(fullTestDir, "file2.txt"), "");
            AssetDatabase.Refresh();
            string yaml = @"
- action: list_files
  arguments:
    directory: TempTestDataYaml
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("file1.txt", result);
            StringAssert.Contains("file2.txt", result);
        }

        [Test]
        public void ListFiles_NotFound_Error()
        {
            string yaml = @"
- action: list_files
  arguments:
    directory: NoDirYaml
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Directory not found", result);
        }

        [Test]
        public void ReadFile_Success()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "read.txt"), "content");
            AssetDatabase.Refresh();
            string yaml = @"
- action: read_file
  arguments:
    file_path: TempTestDataYaml/read.txt
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("content", result);
        }

        [Test]
        public void ReadFile_NotFound_Error()
        {
            string yaml = @"
- action: read_file
  arguments:
    file_path: TempTestDataYaml/nonexistent.txt
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("File not found", result);
        }

        [Test]
        public void InvalidYaml_MissingAction_Skipped()
        {
            string yaml = @"
- arguments:
    file_path: TempTestDataYaml/x.txt
";
            var patches = SimpleYamlParser.Parse(yaml);
            Assert.GreaterOrEqual(patches.Count, 0);
            if (patches.Count > 0)
            {
                string result = ApplySingleYamlPatch(yaml);
                Assert.IsNull(result);
            }
        }

        [Test]
        public void InvalidYaml_NoArguments_Error()
        {
            string yaml = @"
- action: create_file
";
            LogAssert.Expect(LogType.Error, "[1] create_file: ERROR - Missing arguments");
            var patches = SimpleYamlParser.Parse(yaml);
            PatchApplier.ApplyPatches(patches);
        }

        [Test]
        public void UnknownAction_Error()
        {
            string yaml = @"
- action: unknown_action
  arguments:
    file_path: x
";
            LogAssert.Expect(LogType.Error, "[1] unknown_action: ERROR - Unknown action: unknown_action");
            ApplySingleYamlPatch(yaml);
        }

        [Test]
        public void MultilineContent_PreservesFormatting()
        {
            string yaml = @"
- action: create_file
  arguments:
    file_path: TempTestDataYaml/multiline.txt
    file_content: |
      line1
      line2
        indented line
      line3
";
            ApplySingleYamlPatch(yaml);
            string content = File.ReadAllText(Path.Combine(fullTestDir, "multiline.txt"));
            string[] lines = content.Replace("\r\n", "\n").Split('\n');
            Assert.AreEqual("line1", lines[0]);
            Assert.AreEqual("line2", lines[1]);
            Assert.AreEqual("  indented line", lines[2]);
            Assert.AreEqual("line3", lines[3]);
        }

        [Test]
        public void CreateFile_BlockedPathYaml_AccessDenied()
        {
            string yaml = @"
- action: create_file
  arguments:
    file_path: Plugins/forbidden_yaml.txt
    file_content: blocked
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Access denied", result);
        }

        [Test]
        public void AllowedList_Whitelist_Yaml_SuccessAndDenied()
        {
            PatchApplier.AllowedPathPrefixes.Clear();
            PatchApplier.AllowedPathPrefixes.Add("TempTestDataYaml/");

            string yamlOk = @"
- action: create_file
  arguments:
    file_path: TempTestDataYaml/ok_yaml.txt
    file_content: safe
";
            string resultOk = ApplySingleYamlPatch(yamlOk);
            StringAssert.Contains("File created", resultOk);

            string yamlDenied = @"
- action: create_file
  arguments:
    file_path: Plugins/denied_yaml.txt
    file_content: nope
";
            string resultDenied = ApplySingleYamlPatch(yamlDenied);
            StringAssert.Contains("Access denied", resultDenied);
        }

        // ------------------------------------------------------------
        // Новые YAML-тесты для file_exists, insert_before_class_end, insert_after_line
        // ------------------------------------------------------------

        [Test]
        public void FileExists_Yaml_True()
        {
            File.WriteAllText(Path.Combine(fullTestDir, "exists.txt"), "");
            AssetDatabase.Refresh();
            string yaml = @"
- action: file_exists
  arguments:
    file_path: TempTestDataYaml/exists.txt
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("true", result);
        }

        [Test]
        public void FileExists_Yaml_False()
        {
            string yaml = @"
- action: file_exists
  arguments:
    file_path: TempTestDataYaml/no.txt
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("false", result);
        }

        [Test]
        public void InsertBeforeClassEnd_Yaml_Success()
        {
            string classCode = @"using UnityEngine;
public class TestClass : MonoBehaviour
{
    void Start() { }
}";
            File.WriteAllText(Path.Combine(fullTestDir, "TestClass.cs.txt"), classCode);

            string yaml = @"
- action: insert_before_class_end
  arguments:
    file_path: TempTestDataYaml/TestClass.cs.txt
    class_name: TestClass
    file_content: |
      void OnEnable()
      {
          Debug.Log(""Enabled"");
      }
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Inserted code before end of class", result);
            string updated = File.ReadAllText(Path.Combine(fullTestDir, "TestClass.cs.txt"));
            StringAssert.Contains("void OnEnable()", updated);
        }

        [Test]
        public void InsertAfterLine_Yaml_Success()
        {
            string original = @"line1
line2
target line
line3";
            File.WriteAllText(Path.Combine(fullTestDir, "insert_test.txt"), original);
            AssetDatabase.Refresh();

            string yaml = @"
- action: insert_after_line
  arguments:
    file_path: TempTestDataYaml/insert_test.txt
    search_line_contains: target
    file_content: INSERTED
";
            string result = ApplySingleYamlPatch(yaml);
            StringAssert.Contains("Inserted code after line containing 'target'", result);
            string updated = File.ReadAllText(Path.Combine(fullTestDir, "insert_test.txt"));
            string[] updatedLines = updated.Replace("\r\n", "\n").Split('\n');
            bool found = false;
            for (int i = 0; i < updatedLines.Length - 1; i++)
            {
                if (updatedLines[i].Trim() == "target line" &&
                    updatedLines[i+1].Trim() == "INSERTED")
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "Expected 'INSERTED' after 'target line'");
        }
    }
}
