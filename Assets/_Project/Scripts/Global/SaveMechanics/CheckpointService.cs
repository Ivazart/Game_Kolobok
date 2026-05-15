using System;

namespace Global
{
    public class CheckpointService
    {
        public int LastCheckPointID { get; internal set; } = -1;
        public event Action<int> OnSavedJumpsChanged;
        public event Action OnNewCheckpointReached;

        private SaveData saveData;
        private SaveHandler saveHandler;
        private ILevelOrderService levelOrderService;
        private ISceneContext sceneContext;

        public CheckpointService(SaveData saveData, SaveHandler saveHandler,
            ISceneContext sceneContext, ILevelOrderService levelOrderService)
        {
            this.saveData = saveData;
            this.saveHandler = saveHandler;
            this.sceneContext = sceneContext;
            this.levelOrderService = levelOrderService;
        }

        public void NewCheckPointReached(int index)
        {
            if (!levelOrderService.IsLevel(sceneContext.CurrentScene))
                return;

            saveData.LastCheckpointData.LevelName = sceneContext.CurrentScene;
            saveData.LastCheckpointData.Checkpoint = index;
            saveData.LevelDatas[sceneContext.CurrentScene].LastCheckpoint = saveData.LastCheckpointData;
            saveHandler.Save(saveData);
            LastCheckPointID = index;
            OnNewCheckpointReached?.Invoke();
        }

        public void SaveJumpCounter(int value)
        {
            saveData.LastCheckpointData.Jumps = value;
            saveData.LevelDatas[sceneContext.CurrentScene].LastCheckpoint.Jumps = value;
            saveHandler.Save(saveData);
            OnSavedJumpsChanged?.Invoke(value);
        }

        public void ClearLevelProgress()
        {
            saveData.LastCheckpointData.Checkpoint = -1;
            saveData.LastCheckpointData.Jumps = 0;
            saveData.LastCheckpointData.Progress = 0;
            saveData.LevelDatas[sceneContext.CurrentScene].LastCheckpoint = saveData.LastCheckpointData;
            LastCheckPointID = -1;
            saveHandler.Save(saveData);
            OnSavedJumpsChanged?.Invoke(saveData.LastCheckpointData.Jumps);
        }
    }
}