using UnityEngine.SceneManagement;

namespace Global
{
    public class SceneController : SingletonBase<SceneController>
    {
        public SceneName CurrentSceneName { get; private set; }
        
        public void LoadScene(SceneName sceneName)
        {
            CurrentSceneName = sceneName;
            SceneManager.LoadScene(sceneName.ToString());
        }

        public void RestartScene()
        {
            LoadScene(CurrentSceneName);
        }
        
    }
}