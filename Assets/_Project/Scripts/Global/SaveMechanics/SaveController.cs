using System;
using System.Collections.Generic;
using System.Linq;
using StartingLab;
using UnityEngine;

namespace Global
{
    public class SaveController : SingletonBase<SaveController>
    {
        public int LastCheckPointID { get; private set; } = 0;
        
        private SaveData saveData = new ();
        private SaveHandler saveHandler = new ();
        private SceneController sceneController => SceneController.Instance;
        protected override void Awake()
        {
            base.Awake();
            saveData = saveHandler.Load();
        }
        
        public void LevelCompleted()
        {
            var scene = sceneController.CurrentSceneName;
            
            if (saveData.LevelDatas.ContainsKey(scene) == false)
            {
                Debug.LogError("Level completed without any level data");
                return;
            }
            
            saveData.LevelDatas[scene].IsFinished = true;
            saveData.LevelDatas[scene].JumpRecord = GetJumpRecord();
            saveData.LevelDatas[scene].LastCheckpoint.Checkpoint = 0;
            saveData.LevelDatas[scene].LastCheckpoint.Jumps = 0;
            saveHandler.Save(saveData);
            LastCheckPointID = 0;
        }
        
        public void NewCheckPointReached(int index)
        {
            var scene = sceneController.CurrentSceneName;
            
            var data = new LastCheckpointData()
            {
                Checkpoint = index, 
                Jumps = 0, 
                LevelName = scene
            };
            
            if (saveData.LevelDatas.ContainsKey(scene))
                saveData.LevelDatas[scene].LastCheckpoint = data;
            else
            {
                var levelData = new LevelData()
                {
                    IsFinished = false,
                    JumpRecord = 0,
                    LevelName = scene,
                    LastCheckpoint = data
                };
                saveData.LevelDatas.Add(scene, levelData);
            }
            saveData.LastCheckpointData = data;
            saveHandler.Save(saveData);
            LastCheckPointID = index;
        }

        public void LoadLastSave()
        {
            var scene = saveData.LastCheckpointData.LevelName;
            LastCheckPointID = saveData.LastCheckpointData.Checkpoint;
            Debug.Log($"Scene loaded {scene}, checkpoint {LastCheckPointID}");
            if (scene != sceneController.CurrentSceneName) 
                sceneController.LoadScene(scene);
        }

        private int GetJumpRecord()
        {
            return 0;
        }
    }
}