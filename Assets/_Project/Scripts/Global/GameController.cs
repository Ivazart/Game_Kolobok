using System;
using UnityEngine;

namespace Global
{
    public class GameController: SingletonBase<GameController>
    {
        public event Action<DeathType> OnPlayerDeath; 
        private SaveController saveController => SaveController.Instance;
        private SceneController sceneController => SceneController.Instance;
        private void Start()
        {
            saveController.LoadLastSave();
        }

        public void PlayerDeath(DeathType deathType)
        {
            Debug.Log($"Player death {deathType}");
            OnPlayerDeath?.Invoke(deathType);
        }

        public void PlayerDeathAnimationFinished()
        {
            sceneController.RestartScene();
        }
        
    }
}