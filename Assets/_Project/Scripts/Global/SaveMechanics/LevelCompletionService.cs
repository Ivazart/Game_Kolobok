using System;
using UnityEngine;

namespace Global
{
    public class LevelCompletionService
    {
        public event Action OnLevelFinished;
        public event Action<int> OnSavedJumpsChanged;

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

            bool isFirstTime = CurrentSceneFinished(scene);


            SceneName nextScene = isFirstTime ? GetNextLevel(scene) : SceneName.SelectStageScene;
            
            saveData.LastCheckpointData.LevelName = nextScene;
            saveData.LastCheckpointData.Checkpoint = -1;
            saveHandler.Save(saveData);
            checkpointService.LastCheckPointID = saveData.LastCheckpointData.Checkpoint;
            sceneContext.LoadScene(nextScene);
            OnSavedJumpsChanged?.Invoke(0);
            OnLevelFinished?.Invoke();
        }

        private SceneName GetNextLevel(SceneName scene)
        {
            var nextLevel = levelOrderService.GetNextLevel(scene);
            if (nextLevel != scene)
            {
                var nextLevelData = saveData.LevelDatas[nextLevel];
                nextLevelData.IsOpen = true;
            }

            return nextLevel;
        }

        private bool CurrentSceneFinished(SceneName scene)
        {
            bool isFirstTime = !saveData.LevelDatas[scene].IsFinished;
            saveData.LevelDatas[scene].IsFinished = true;
            saveData.LevelDatas[scene].JumpRecord = GetJumpRecord(scene);
            saveData.LevelDatas[scene].LastCheckpoint.Checkpoint = -1;
            saveData.LevelDatas[scene].LastCheckpoint.Jumps = 0;
            return isFirstTime;
        }

        private int GetJumpRecord(SceneName scene)
        {
            int jumps = saveData.LastCheckpointData?.Jumps ?? 0;
            return Math.Min(jumps, saveData.LevelDatas[scene].JumpRecord);
        }
    }
}