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
            if (Enum.TryParse<SceneName>(scene, out SceneName sceneEnum) == false)
            {
                Debug.LogError($"scene {scene} is not in SceneName enum. Please add scene in enum");
                return;
            }

            CurrentSceneName = sceneEnum;
            Debug.Log("start scene name:" + CurrentSceneName);
        }

        public void LoadScene(SceneName sceneName)
        {
            Debug.Log("scene name:" + CurrentSceneName);
            CurrentSceneName = sceneName;
            SceneManager.LoadScene(sceneName.ToString());
        }

        public void RestartScene()
        {
            LoadScene(CurrentSceneName);
        }
    }
}