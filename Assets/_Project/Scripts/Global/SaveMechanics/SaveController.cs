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
        public int LastCheckPointID { get; private set; } = -1;
        public event Action<int> OnJumpCounterChanged;
        public event Action OnTutorFinished;
        public SaveData SaveData => saveData;
        public int JumpCounter => GetJumps();

        [SerializeField] private SceneImageDatabase sceneImageDatabase;
        
        private SaveData saveData = new();
        private SaveHandler saveHandler = new();
        private SceneController sceneController => SceneController.Instance;
        private SceneName scene => sceneController.CurrentSceneName;

        protected override void Awake()
        {
            base.Awake();
            saveData = saveHandler.Load();
            LoadLastSave();
        }

        private int GetJumps()
        {
            if (saveData == null || saveData.LastCheckpointData == null)
                return 0;
            return saveData.LastCheckpointData.Jumps;
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
            OnJumpCounterChanged?.Invoke(0);
        }

        public void IncreaseJumpCounter()
        {
            int jumps = saveData.LastCheckpointData.Jumps + 1;
            saveData.LastCheckpointData.Jumps = jumps;
            saveData.LevelDatas[scene].LastCheckpoint.Jumps = jumps;
            saveHandler.Save(saveData);
            OnJumpCounterChanged?.Invoke(jumps);
        }

        public void NewCheckPointReached(int index)
        {
            saveData.LastCheckpointData.LevelName = scene;
            saveData.LastCheckpointData.Checkpoint = index;
            saveData.LevelDatas[scene].LastCheckpoint = saveData.LastCheckpointData;
            saveHandler.Save(saveData);
            LastCheckPointID = index;
        }

        public void ClearLevelProgress()
        {
            saveData.LastCheckpointData.Checkpoint = 0;
            saveData.LastCheckpointData.Jumps = 0;
            saveData.LastCheckpointData.Progress = 0;
            saveData.LevelDatas[scene].LastCheckpoint = saveData.LastCheckpointData;
            LastCheckPointID = -1;
            saveHandler.Save(saveData);
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
            OnJumpCounterChanged?.Invoke(saveData.LastCheckpointData.Jumps);
        }
        
        private int GetJumpRecord()
        {
            return 0;
        }
    }
}