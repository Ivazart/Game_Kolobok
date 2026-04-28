using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scriptable;
using StartingLab;
using UnityEngine;

namespace Global
{
    public class SaveController : SingletonBase<SaveController>
    {
        [SerializeField] private SceneImageDatabase sceneImageDatabase;
        
        public int LastCheckPointID { get; private set; } = -1;
        public event Action<int> OnJumpCounterChanged;
        public event Action<int> OnSavedJumpsChanged;
        public event Action OnNewCheckpointReached;
        public event Action OnLevelFinished;
        public event Action OnTutorFinished;
        public SaveData SaveData => saveData;
        public int JumpCounter => GetJumps();

        
        private SaveData saveData = new();
        private SaveHandler saveHandler = new();
        private SceneController sceneController => SceneController.Instance;
        private SceneName scene => sceneController.CurrentSceneName;

        protected override void Awake()
        {
            base.Awake();
            saveData = saveHandler.Load();
        }

        private void Start()
        {
            LoadLastSave();
        }

        private int GetJumps()
        {
            return saveData?.LastCheckpointData?.Jumps ?? 0;
        }
        
        public Sprite GetSpriteByScene(SceneName sceneType)
        {
            return sceneImageDatabase.GetSpriteByScene(sceneType);
        }

        public void LevelCompleted()
        {
            if (saveData.LevelDatas.ContainsKey(scene) == false)
            {
                Debug.LogError("Level completed without any level data");
                return;
            }

            var nextLevel = LevelOrder.GetNextLevel(scene);
            if (nextLevel != scene)
            { 
                var nextLevelData = saveHandler.CreateLevel(nextLevel);
                if (saveData.LevelDatas.ContainsKey(nextLevel) == false)
                    saveData.LevelDatas.Add(nextLevel, nextLevelData); 
            }
           
            saveData.LevelDatas[scene].IsFinished = true;
            saveData.LevelDatas[scene].JumpRecord = GetJumpRecord();
            saveData.LevelDatas[scene].LastCheckpoint.Checkpoint = 0;
            saveData.LevelDatas[scene].LastCheckpoint.Jumps = 0;
            saveData.LastCheckpointData = saveData.LevelDatas[scene].LastCheckpoint;
            saveData.LastCheckpointData.LevelName = nextLevel;
            saveHandler.Save(saveData);
            LastCheckPointID = -1;
            sceneController.LoadScene(nextLevel);
            OnLevelFinished?.Invoke();
        }

        public void SaveJumpCounter(int value)
        {
            saveData.LastCheckpointData.Jumps = value;
            saveData.LevelDatas[scene].LastCheckpoint.Jumps = value;
            saveHandler.Save(saveData);
            OnSavedJumpsChanged?.Invoke(value);
        }


        public void NewCheckPointReached(int index)
        {
            saveData.LastCheckpointData.LevelName = scene;
            saveData.LastCheckpointData.Checkpoint = index;
            saveData.LevelDatas[scene].LastCheckpoint = saveData.LastCheckpointData;
            saveHandler.Save(saveData);
            LastCheckPointID = index;
            OnNewCheckpointReached?.Invoke();
        }

        public void ClearLevelProgress()
        {
            saveData.LastCheckpointData.Checkpoint = 0;
            saveData.LastCheckpointData.Jumps = 0;
            saveData.LastCheckpointData.Progress = 0;
            saveData.LevelDatas[scene].LastCheckpoint = saveData.LastCheckpointData;
            LastCheckPointID = -1;
            saveHandler.Save(saveData);
            OnSavedJumpsChanged?.Invoke(saveData.LastCheckpointData.Jumps);
        }

        public void TutorFinished()
        {
            if ( saveData.IsTutorFinished )
                return;
            saveData.IsTutorFinished = true;
            saveHandler.Save(saveData);
            OnTutorFinished?.Invoke();
        }

        private void LoadLastSave()
        {
            var scene = saveData.LastCheckpointData.LevelName;
            LastCheckPointID = saveData.LastCheckpointData.Checkpoint;
            Debug.Log($"Scene loaded {scene}, checkpoint {LastCheckPointID}");
            if (scene != sceneController.CurrentSceneName)
                sceneController.LoadScene(scene);
            OnSavedJumpsChanged?.Invoke(saveData.LastCheckpointData.Jumps);
        }
        
        private int GetJumpRecord()
        {
            return 0;
        }
    }
}