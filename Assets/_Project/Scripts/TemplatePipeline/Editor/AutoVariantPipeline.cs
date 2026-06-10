using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

public class AutoVariantPipeline : ISceneTemplatePipeline
{
    public bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset) => true;

    public void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName) { }

    public void AfterTemplateInstantiation(
        SceneTemplateAsset sceneTemplateAsset,
        Scene scene,
        bool isAdditive,
        string scenePath)
    {
        var markers = Object.FindObjectsByType<ScenePrefabVariant>(FindObjectsSortMode.None);
        if (markers.Length == 0)
        {
            Debug.Log("No ScenePrefabVariant markers found.");
            return;
        }

        foreach (var marker in markers)
        {
            GameObject go = marker.gameObject;

            if (!PrefabUtility.IsPartOfPrefabInstance(go))
            {
                Debug.LogWarning($"{go.name} is not a prefab instance, skipping.");
                continue;
            }

            // Определяем исходный префаб
            GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (sourcePrefab == null)
            {
                Debug.LogWarning($"No source prefab for {go.name}, skipping.");
                continue;
            }

            string sourcePath = AssetDatabase.GetAssetPath(sourcePrefab);
            string folder = Path.GetDirectoryName(sourcePath);

            // Генерируем уникальный временный суффикс (8 символов GUID)
            string tempId = Guid.NewGuid().ToString().Substring(0, 8);
            string variantFileName = $"{sourcePrefab.name}_Temp_{tempId}.prefab";
            string variantPath = Path.Combine(folder, variantFileName);
            variantPath = AssetDatabase.GenerateUniqueAssetPath(variantPath);

            // Удаляем компонент-маркер из объекта, чтобы он не попал в вариант и в сцену
            Object.DestroyImmediate(marker);

            // Создаём вариант и сразу подключаем объект в сцене к новому варианту
            PrefabUtility.SaveAsPrefabAssetAndConnect(go, variantPath, InteractionMode.AutomatedAction);

            Debug.Log($"Created variant: {variantPath}");
        }

        // Сохраняем сцену, чтобы зафиксировать новые варианты
        if (!string.IsNullOrEmpty(scenePath) && File.Exists(scenePath))
        {
            EditorSceneManager.SaveScene(scene, scenePath);
        }
        else
        {
            // Если сцена ещё не сохранена — вызываем диалог (на всякий случай)
            string path = EditorUtility.SaveFilePanelInProject("Save new scene", scene.name, "unity", "Choose scene name");
            if (!string.IsNullOrEmpty(path))
                EditorSceneManager.SaveScene(scene, path);
        }
    }
}