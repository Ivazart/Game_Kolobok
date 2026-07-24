using System;
using UnityEngine;

namespace Global
{
    public class GameController: SingletonBase<GameController>
    {
        public event Action<DeathType> OnPlayerDeath; 
        public event Action OnLevelRestarted; 
        public event Action OnCheckpointRestarted;

        public event Action OnDragStarted;
        public event Action OnDragEnded;
        
        public event Action  OnGameRestarted;
        public event Action OnSaveLoaded; 
        private SaveController saveController => SaveController.Instance;
        private SceneController sceneController => SceneController.Instance;
    

        public void PlayerDeath(DeathType deathType)
        {
            Debug.Log($"Player death {deathType}");
            OnPlayerDeath?.Invoke(deathType);
        }

        public void PlayerDeathAnimationFinished()
        {
            sceneController.RestartScene();
        }

        public void RestartLevel()
        {
            saveController.ClearLevelProgress();
            sceneController.RestartScene();
            OnLevelRestarted?.Invoke();
        }

        public void RestartCheckpoint()
        {
            sceneController.RestartScene();
            OnCheckpointRestarted?.Invoke();
        }
        
        public void RestartGame()
        {
            saveController.DeleteSave();
            OnGameRestarted?.Invoke();
            //sceneController.LoadScene(SceneName.StartLab);
        }

        public void LoadLevelFromSaves(SceneName levelName)
        {
            sceneController.LoadScene(levelName);
            OnSaveLoaded?.Invoke();
        }
        
        public void LoadSelectLevel()
        {
            OnSaveLoaded?.Invoke();
        }

        public void SetDragHandler(DragHandler handler)
        {
            handler.OnDragEnded += () =>  OnDragEnded?.Invoke();
            handler.OnDragStarted += () => OnDragStarted?.Invoke();
        }
        
    }
}