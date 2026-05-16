using System;
using UnityEngine;

namespace Global
{
    public class JumpsCounterController : SingletonBase<JumpsCounterController>
    {
        public int Jumps => savedJumps + localJumps;

        public event Action OnJumpsChanged;
        private SaveController saveController => SaveController.Instance;
        private GameController gameController => GameController.Instance;
        
        private int localJumps;
        private int savedJumps;

        protected override void Awake()
        {
            base.Awake();
            gameController.OnPlayerDeath += OnPlayerDeath;
            gameController.OnLevelRestarted += ClearLocalJumps;
            gameController.OnCheckpointRestarted  += ClearLocalJumps;
            gameController.OnSaveLoaded += ClearLocalJumps;
            gameController.OnGameRestarted += ClearLocalJumps;
            saveController.OnSavedJumpsChanged += SaveControllerOnSavedJumpsChanged;
            saveController.OnNewCheckpointReached += SaveController_OnNewCheckpointReached;
            saveController.OnLevelFinished += ClearLocalJumps;
            
        }
        
        public void IncreaseJumpCounter()
        {
            localJumps++;
            OnJumpsChanged?.Invoke();
        }

        private void SaveController_OnNewCheckpointReached()
        {
            saveController.SaveJumpCounter(Jumps);
        }

        private void SaveControllerOnSavedJumpsChanged(int newSavedJumps)
        {
            savedJumps = newSavedJumps;
            localJumps = 0;
            OnJumpsChanged?.Invoke();
        }

        private void OnPlayerDeath(DeathType type)
        {
            ClearLocalJumps();
        }

        private void ClearLocalJumps()
        {
            localJumps = 0;
            OnJumpsChanged?.Invoke(); 
        }
    }
}