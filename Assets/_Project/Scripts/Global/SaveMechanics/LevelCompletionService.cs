using System;
using UnityEngine;

namespace Global
{
    public class LevelCompletionService
    {
        public event Action OnLevelFinished;

        private readonly SaveData saveData;
        private readonly SaveHandler saveHandler;
        private readonly ISceneContext sceneContext;
        private readonly ILevelOrderService levelOrderService;
        private readonly CheckpointService checkpointService;

        public LevelCompletionService(SaveData saveData, SaveHandler saveHandler,
            ISceneContext sceneContext, ILevelOrderService levelOrderService,
            CheckpointService checkpointService)
        {
            this.saveData = saveData;
            this.saveHandler = saveHandler;
            this.sceneContext = sceneContext;
            this.levelOrderService = levelOrderService;
            this.checkpointService = checkpointService;
        }

        public void LevelCompleted()
        {
            var scene = sceneContext.CurrentScene;
            if (!saveData.LevelDatas.ContainsKey(scene))
            {
                Debug.LogError("Level completed without any level data");
                return;
            }

            var nextLevel = levelOrderService.GetNextLevel(scene);
            if (nextLevel != scene)
            {
                var nextLevelData = saveData.LevelDatas[nextLevel];
                nextLevelData.IsOpen = true;
            }
            
            saveData.LevelDatas[scene].IsFinished = true;
            saveData.LevelDatas[scene].JumpRecord = GetJumpRecord(scene);
            saveData.LevelDatas[scene].LastCheckpoint.Checkpoint = -1;
            saveData.LevelDatas[scene].LastCheckpoint.Jumps = 0;
            saveData.LastCheckpointData = saveData.LevelDatas[nextLevel].LastCheckpoint;
            saveData.LastCheckpointData.LevelName = nextLevel;
            saveHandler.Save(saveData);

            checkpointService.LastCheckPointID = -1;
            sceneContext.LoadScene(nextLevel);
            OnLevelFinished?.Invoke();
        }

        private int GetJumpRecord(SceneName scene)
        {
            int jumps = saveData.LastCheckpointData?.Jumps ?? 0;
            return Math.Min(jumps, saveData.LevelDatas[scene].JumpRecord);
        }
    }
}