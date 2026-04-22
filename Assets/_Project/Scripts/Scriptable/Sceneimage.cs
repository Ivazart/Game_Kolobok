using UnityEngine;
using UnityEngine;
using System.Collections.Generic;
using Global;

namespace _Project.Scriptable
{
    [CreateAssetMenu(fileName = "SceneImageDatabase", menuName = "Scriptable/Scene Image Database")]
    public class SceneImageDatabase : ScriptableObject
    {
        public List<SceneImagePair> sceneImagePairs = new List<SceneImagePair>();

        // Метод для получения спрайта по типу сцены (необязательно, но удобно)
        public Sprite GetSpriteByScene(SceneName sceneType)
        {
            foreach (var pair in sceneImagePairs)
            {
                if (pair.sceneType == sceneType)
                    return pair.sceneImage;
            }
            Debug.LogWarning($"Sprite {sceneType} not found!");
            return null;
        }
    }
    
    [System.Serializable]
    public class SceneImagePair
    {
        public SceneName sceneType;
        public Sprite sceneImage; // Sprite используется для Image в UI
    }
}