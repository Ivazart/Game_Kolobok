using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global
{
    public class SceneController : SingletonBase<SceneController>
    {
        public SceneName CurrentSceneName { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            string scene = SceneManager.GetActiveScene().name;
            if (Enum.TryParse(scene, out SceneName sceneEnum) == false)
            {
                Debug.LogError($"scene {scene} is not in SceneName enum. Please add scene in enum");
                return;
            }

            CurrentSceneName = sceneEnum;
            Debug.Log("Start scene: " + CurrentSceneName);
        }

        public void LoadScene(SceneName sceneName)
        {
            Debug.Log("Load scene: " + CurrentSceneName);
            CurrentSceneName = sceneName;
            SceneManager.LoadScene(sceneName.ToString());
            Time.timeScale = 1f;
        }

        public void RestartScene()
        {
            Debug.Log("Reload scene: " + CurrentSceneName);
            LoadScene(CurrentSceneName);
        }
    }
}